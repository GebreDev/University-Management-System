using AutoMapper;
using UniversityManagementSystem.Application.DTOs.CourseMaterial;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class CourseMaterialService : ICourseMaterialService
    {
        private readonly ICourseMaterialRepository _materialRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IFileService _fileService;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public CourseMaterialService(
            ICourseMaterialRepository materialRepo,
            ICourseRepository courseRepo,
            IEnrollmentRepository enrollmentRepo,
            IFileService fileService,
            IAuditLogService auditLogService,
            IMapper mapper)
        {
            _materialRepo = materialRepo;
            _courseRepo = courseRepo;
            _enrollmentRepo = enrollmentRepo;
            _fileService = fileService;
            _auditLogService = auditLogService;
            _mapper = mapper;
        }

        public async Task<CourseMaterialViewDto> UploadAsync(CourseMaterialUploadDto dto, string uploaderUserId)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new CustomException("Please choose a file to upload.");

            var course = await _courseRepo.GetByIdAsync(dto.CourseId)
                ?? throw new NotFoundException(nameof(Course), dto.CourseId);

            string savedPath;
            try
            {
                savedPath = (await _fileService.SaveMaterialFileAsync(dto.File)) ?? string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                throw new CustomException(ex.Message);
            }

            if (string.IsNullOrEmpty(savedPath))
                throw new CustomException("The file could not be saved. Please try again.");

            try
            {
                var material = new CourseMaterial
                {
                    Title = dto.Title.Trim(),
                    Description = dto.Description?.Trim(),
                    FileName = dto.File.FileName,
                    FilePath = savedPath,
                    FileSize = dto.File.Length,
                    FileType = Path.GetExtension(dto.File.FileName).TrimStart('.').ToLowerInvariant(),
                    CourseId = dto.CourseId,
                    UploadedById = uploaderUserId
                };

                await _materialRepo.AddAsync(material);
                await _materialRepo.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    uploaderUserId,
                    string.Empty,
                    "Instructor",
                    "Create",
                    "CourseMaterial",
                    material.Id,
                    $"Uploaded course material '{material.Title}' for course {course.Code}.");

                return MapToView(material, course);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception)
            {
                // Do not leave orphan files behind when the DB write fails.
                _fileService.DeleteFile(savedPath);
                throw;
            }
        }

        public async Task<CourseMaterialViewDto> UpdateAsync(CourseMaterialUpdateDto dto, string uploaderUserId)
        {
            var material = await _materialRepo.GetByIdWithDetailsAsync(dto.Id)
                ?? throw new NotFoundException(nameof(CourseMaterial), dto.Id);

            material.Title = dto.Title.Trim();
            material.Description = dto.Description?.Trim();

            // Optional file replacement with the same extension rules.
            if (dto.File != null && dto.File.Length > 0)
            {
                string newPath;
                try
                {
                    newPath = await _fileService.SaveMaterialFileAsync(dto.File)
                        ?? throw new CustomException("The file could not be saved. Please try again.");
                }
                catch (InvalidOperationException ex)
                {
                    throw new CustomException(ex.Message);
                }

                var oldPath = material.FilePath;
                var oldFileName = material.FileName;
                var oldSize = material.FileSize;
                var oldType = material.FileType;

                material.FileName = dto.File.FileName;
                material.FilePath = newPath;
                material.FileSize = dto.File.Length;
                material.FileType = Path.GetExtension(dto.File.FileName).TrimStart('.').ToLowerInvariant();

                try
                {
                    await _materialRepo.UpdateAsync(material);
                    await _materialRepo.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // Roll the entity back to the old file metadata before rethrowing.
                    material.FileName = oldFileName;
                    material.FilePath = oldPath;
                    material.FileSize = oldSize;
                    material.FileType = oldType;
                    _fileService.DeleteFile(newPath);
                    throw;
                }

                _fileService.DeleteFile(oldPath);
            }
            else
            {
                await _materialRepo.UpdateAsync(material);
                await _materialRepo.SaveChangesAsync();
            }

            await _auditLogService.LogAsync(
                uploaderUserId,
                string.Empty,
                "Instructor",
                "Update",
                "CourseMaterial",
                material.Id,
                $"Updated course material '{material.Title}'.");

            return MapToView(material, material.Course);
        }

        public async Task DeleteAsync(int id, string requesterUserId, bool isAdmin)
        {
            var material = await _materialRepo.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException(nameof(CourseMaterial), id);

            if (!isAdmin && !string.Equals(material.UploadedById, requesterUserId, StringComparison.Ordinal))
                throw new Exceptions.UnauthorizedAccessException("You can only delete materials you uploaded.");

            _materialRepo.Delete(material);
            await _materialRepo.SaveChangesAsync();
            _fileService.DeleteFile(material.FilePath);

            await _auditLogService.LogAsync(
                requesterUserId,
                string.Empty,
                isAdmin ? "Admin" : "Instructor",
                "Delete",
                "CourseMaterial",
                id,
                $"Deleted course material '{material.Title}' from course {material.Course?.Code}.");
        }

        public async Task<CourseMaterialViewDto> GetByIdAsync(int id, string requesterUserId, bool isAdmin, int? studentId = null)
        {
            var material = await _materialRepo.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException(nameof(CourseMaterial), id);

            // Access: admins, the uploading instructor, or students enrolled in the course.
            var isUploader = string.Equals(material.UploadedById, requesterUserId, StringComparison.Ordinal);
            if (!isAdmin && !isUploader)
            {
                var enrolled = studentId.HasValue
                    && await _enrollmentRepo.IsStudentEnrolledInCourseAsync(studentId.Value, material.CourseId);

                if (!enrolled)
                    throw new Exceptions.UnauthorizedAccessException("You do not have access to this material.");
            }

            return MapToView(material, material.Course);
        }

        public async Task<IEnumerable<CourseMaterialListDto>> GetForInstructorAsync(string instructorUserId)
        {
            var materials = await _materialRepo.GetByUploaderIdAsync(instructorUserId);
            return (materials ?? Enumerable.Empty<CourseMaterial>())
                .Select(MapToList);
        }

        public async Task<IEnumerable<CourseMaterialListDto>> GetForStudentAsync(int studentId)
        {
            // Materials for the courses the student is actively enrolled in.
            var enrollments = await _enrollmentRepo.GetEnrollmentsByStudentIdAsync(studentId);
            var courseIds = enrollments
                .Where(e => e.Status == Domain.Enums.EnrollmentStatus.Approved)
                .Select(e => e.CourseId)
                .Distinct()
                .ToList();

            if (courseIds.Count == 0)
                return Enumerable.Empty<CourseMaterialListDto>();

            var materials = await _materialRepo.GetByCourseIdsAsync(courseIds);
            return (materials ?? Enumerable.Empty<CourseMaterial>())
                .Select(MapToList);
        }

        public async Task IncrementDownloadCountAsync(int id)
        {
            var material = await _materialRepo.GetByIdAsync(id);
            if (material == null)
                return;

            material.DownloadCount++;
            await _materialRepo.UpdateAsync(material);
            await _materialRepo.SaveChangesAsync();
        }

        public async Task<bool> CanManageAsync(int materialId, string userId, bool isAdmin)
        {
            if (isAdmin) return true;
            var material = await _materialRepo.GetByIdAsync(materialId);
            return material != null
                && string.Equals(material.UploadedById, userId, StringComparison.Ordinal);
        }

        public async Task<bool> StudentHasAccessAsync(int materialId, int studentId)
        {
            var material = await _materialRepo.GetByIdAsync(materialId);
            if (material == null) return false;
            return await _enrollmentRepo.IsStudentEnrolledInCourseAsync(studentId, material.CourseId);
        }

        private static CourseMaterialListDto MapToList(CourseMaterial m)
        {
            return new CourseMaterialListDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                FileName = m.FileName,
                FileType = m.FileType,
                FileSize = m.FileSize,
                DownloadCount = m.DownloadCount,
                CreatedAt = m.CreatedAt,
                CourseId = m.CourseId,
                CourseTitle = m.Course?.Title ?? string.Empty,
                CourseCode = m.Course?.Code ?? string.Empty,
                UploadedByName = m.UploadedBy != null
                    ? $"{m.UploadedBy.FirstName} {m.UploadedBy.LastName}".Trim()
                    : string.Empty
            };
        }

        private static CourseMaterialViewDto MapToView(CourseMaterial m, Course? course)
        {
            return new CourseMaterialViewDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                FileName = m.FileName,
                FilePath = m.FilePath,
                FileType = m.FileType,
                FileSize = m.FileSize,
                DownloadCount = m.DownloadCount,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                CourseId = m.CourseId,
                CourseTitle = course?.Title ?? m.Course?.Title ?? string.Empty,
                CourseCode = course?.Code ?? m.Course?.Code ?? string.Empty,
                UploadedById = m.UploadedById,
                UploadedByName = m.UploadedBy != null
                    ? $"{m.UploadedBy.FirstName} {m.UploadedBy.LastName}".Trim()
                    : string.Empty
            };
        }
    }
}
