using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Enrollment;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository repository, IStudentRepository studentRepository,
            ICourseRepository courseRepository, IMapper mapper)
        {
            _repository = repository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EnrollmentListDto>> GetAllEnrollmentsAsync()
        {
            var enrollments = await _repository.GetAllEnrollmentsWithDetailsAsync();
            return _mapper.Map<IEnumerable<EnrollmentListDto>>(enrollments);
        }

        public async Task<EnrollmentViewDto> GetEnrollmentByIdAsync(int id)
        {
            var enrollment = await _repository.GetEnrollmentWithDetailsAsync(id);
            if (enrollment == null)
                throw new NotFoundException(nameof(Enrollment), id);
            return _mapper.Map<EnrollmentViewDto>(enrollment);
        }

        public async Task<EnrollmentViewDto> CreateEnrollmentAsync(EnrollmentCreateDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
                throw new NotFoundException(nameof(Student), dto.StudentId);

            var course = await _courseRepository.GetByIdAsync(dto.CourseId);
            if (course == null)
                throw new NotFoundException(nameof(Course), dto.CourseId);

            if (await IsStudentAlreadyEnrolledAsync(dto.StudentId, dto.CourseId))
                throw new DuplicateException("Student is already enrolled in this course.");

            var enrollment = _mapper.Map<Enrollment>(dto);
            enrollment.EnrollmentDate = DateTime.UtcNow;
            enrollment.Status = EnrollmentStatus.Pending;
            enrollment.RejectionReason = null;

            await _repository.AddAsync(enrollment);
            await _repository.SaveChangesAsync();

            return _mapper.Map<EnrollmentViewDto>(enrollment);
        }

        public async Task UpdateEnrollmentAsync(EnrollmentUpdateDto dto)
        {
            var enrollment = await _repository.GetEnrollmentWithDetailsAsync(dto.Id);
            if (enrollment == null)
                throw new NotFoundException(nameof(Enrollment), dto.Id);

            if (!string.IsNullOrEmpty(dto.Status))
            {
                if (Enum.TryParse<EnrollmentStatus>(dto.Status, true, out var newStatus))
                {
                    enrollment.Status = newStatus;
                    if (newStatus == EnrollmentStatus.Approved)
                        enrollment.ApprovalDate = DateTime.UtcNow;
                    else if (newStatus == EnrollmentStatus.Rejected)
                        enrollment.RejectionReason = dto.RejectionReason ?? "No reason provided.";
                }
            }

            await _repository.UpdateAsync(enrollment);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException(nameof(Enrollment), id);

            _repository.Delete(enrollment);
            await _repository.SaveChangesAsync();
        }

        public async Task ApproveEnrollmentAsync(int id)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException(nameof(Enrollment), id);

            enrollment.Status = EnrollmentStatus.Approved;
            enrollment.ApprovalDate = DateTime.UtcNow;
            await _repository.UpdateAsync(enrollment);
            await _repository.SaveChangesAsync();
        }

        public async Task RejectEnrollmentAsync(int id, string reason)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            if (enrollment == null)
                throw new NotFoundException(nameof(Enrollment), id);

            enrollment.Status = EnrollmentStatus.Rejected;
            enrollment.RejectionReason = reason ?? "No reason provided.";
            await _repository.UpdateAsync(enrollment);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<EnrollmentListDto>> GetEnrollmentsByStudentAsync(int studentId)
        {
            var enrollments = await _repository.GetEnrollmentsByStudentIdAsync(studentId);
            return _mapper.Map<IEnumerable<EnrollmentListDto>>(enrollments);
        }

        public async Task<IEnumerable<EnrollmentListDto>> GetEnrollmentsByCourseAsync(int courseId)
        {
            var enrollments = await _repository.GetEnrollmentsByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<EnrollmentListDto>>(enrollments);
        }

        public async Task<IEnumerable<EnrollmentListDto>> GetPendingEnrollmentsAsync()
        {
            var enrollments = await _repository.GetPendingEnrollmentsAsync();
            return _mapper.Map<IEnumerable<EnrollmentListDto>>(enrollments);
        }

        public async Task<bool> IsStudentAlreadyEnrolledAsync(int studentId, int courseId)
        {
            return await _repository.IsStudentEnrolledInCourseAsync(studentId, courseId);
        }
    }
}