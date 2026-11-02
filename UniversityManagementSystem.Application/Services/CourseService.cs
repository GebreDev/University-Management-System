using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Course;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IInstructorRepository _instructorRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public CourseService(ICourseRepository repository, IDepartmentRepository departmentRepository,
            IInstructorRepository instructorRepository, IMapper mapper, IAuditLogService auditLogService)
        {
            _repository = repository;
            _departmentRepository = departmentRepository;
            _instructorRepository = instructorRepository;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<CourseListDto>> GetAllCoursesAsync()
        {
            var courses = await _repository.GetAllCoursesWithDetailsAsync();
            return _mapper.Map<IEnumerable<CourseListDto>>(courses);
        }

        public async Task<CourseViewDto> GetCourseByIdAsync(int id)
        {
            var course = await _repository.GetCourseWithDetailsAsync(id);
            if (course == null)
                throw new NotFoundException(nameof(Course), id);
            return _mapper.Map<CourseViewDto>(course);
        }

        public async Task<CourseViewDto> CreateCourseAsync(CourseCreateDto dto)
        {
            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new NotFoundException(nameof(Department), dto.DepartmentId);
            if (!await _instructorRepository.ExistsAsync(dto.InstructorId))
                throw new NotFoundException(nameof(Instructor), dto.InstructorId);

            if (!await IsCourseCodeUniqueAsync(dto.Code))
                throw new DuplicateException($"Course with code '{dto.Code}' already exists.");

            var course = _mapper.Map<Course>(dto);
            await _repository.AddAsync(course);
            await _repository.SaveChangesAsync();

            // The actor is the admin performing the action, not the assigned
            // instructor; pass null userId (AuditLogService resolves the session user).
            await _auditLogService.LogAsync(null, "Admin", "Admin", "Create", "Course", course.Id,
                $"Created course '{dto.Title}' with code {dto.Code}");

            return _mapper.Map<CourseViewDto>(course);
        }

        public async Task UpdateCourseAsync(CourseUpdateDto dto)
        {
            var course = await _repository.GetByIdAsync(dto.Id);
            if (course == null)
                throw new NotFoundException(nameof(Course), dto.Id);

            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new NotFoundException(nameof(Department), dto.DepartmentId);
            if (!await _instructorRepository.ExistsAsync(dto.InstructorId))
                throw new NotFoundException(nameof(Instructor), dto.InstructorId);

            if (!await IsCourseCodeUniqueAsync(dto.Code, dto.Id))
                throw new DuplicateException($"Course with code '{dto.Code}' already exists.");

            _mapper.Map(dto, course);
            await _repository.UpdateAsync(course);
            await _repository.SaveChangesAsync();

            await _auditLogService.LogAsync(null, "Admin", "Admin", "Update", "Course", dto.Id,
                $"Updated course '{course.Title}' with code {course.Code}");
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException(nameof(Course), id);

            await _auditLogService.LogAsync(null, "Admin", "Admin", "Delete", "Course", id,
                $"Deleted course ID {id} - {course.Title}");

            _repository.Delete(course);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> CourseExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByInstructorAsync(int instructorId)
        {
            var courses = await _repository.GetCoursesByInstructorIdAsync(instructorId);
            return _mapper.Map<IEnumerable<CourseListDto>>(courses);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByDepartmentAsync(int departmentId)
        {
            var courses = await _repository.GetCoursesByDepartmentIdAsync(departmentId);
            return _mapper.Map<IEnumerable<CourseListDto>>(courses);
        }

        public async Task<bool> IsCourseCodeUniqueAsync(string code, int? excludeId = null)
        {
            var existing = await _repository.GetByCodeAsync(code);
            if (existing == null) return true;
            if (excludeId.HasValue && existing.Id == excludeId.Value) return true;
            return false;
        }
    }
}