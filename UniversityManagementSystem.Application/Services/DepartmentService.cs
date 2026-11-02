using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Department;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IInstructorRepository _instructorRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public DepartmentService(IDepartmentRepository repository,
                                 IInstructorRepository instructorRepository,
                                 IStudentRepository studentRepository,
                                 ICourseRepository courseRepository,
                                 IMapper mapper,
                                 IAuditLogService auditLogService)
        {
            _repository = repository;
            _instructorRepository = instructorRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<DepartmentListDto>> GetAllDepartmentsAsync()
        {
            var departments = await _repository.GetAllDepartmentsWithDetailsAsync();
            return _mapper.Map<IEnumerable<DepartmentListDto>>(departments);
        }

        public async Task<DepartmentViewDto> GetDepartmentByIdAsync(int id)
        {
            var department = await _repository.GetDepartmentWithDetailsAsync(id);
            if (department == null)
                throw new NotFoundException(nameof(Department), id);
            return _mapper.Map<DepartmentViewDto>(department);
        }

        public async Task<DepartmentViewDto> CreateDepartmentAsync(DepartmentCreateDto dto)
        {
            if (!await IsDepartmentCodeUniqueAsync(dto.Code))
                throw new DuplicateException($"Department with code '{dto.Code}' already exists.");

            var department = _mapper.Map<Department>(dto);
            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            await _auditLogService.LogAsync(null, "Admin", "Admin", "Create", "Department", department.Id,
                $"Created department '{dto.Name}' with code {dto.Code}");

            return _mapper.Map<DepartmentViewDto>(department);
        }

        public async Task UpdateDepartmentAsync(DepartmentUpdateDto dto)
        {
            var department = await _repository.GetByIdAsync(dto.Id);
            if (department == null)
                throw new NotFoundException(nameof(Department), dto.Id);

            if (!await IsDepartmentCodeUniqueAsync(dto.Code, dto.Id))
                throw new DuplicateException($"Department with code '{dto.Code}' already exists.");

            _mapper.Map(dto, department);
            await _repository.UpdateAsync(department);
            await _repository.SaveChangesAsync();

            await _auditLogService.LogAsync(null, "Admin", "Admin", "Update", "Department", dto.Id,
                $"Updated department '{dto.Name}' with code {dto.Code}");
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);
            if (department == null)
                throw new NotFoundException(nameof(Department), id);

            await _auditLogService.LogAsync(null, "Admin", "Admin", "Delete", "Department", id,
                $"Deleted department ID {id} - {department.Name}");

            var students = await _studentRepository.Find(s => s.DepartmentId == id).ToListAsync();
            foreach (var student in students)
            {
                _studentRepository.Delete(student);
            }
            await _studentRepository.SaveChangesAsync();

            var instructors = await _instructorRepository.Find(t => t.DepartmentId == id).ToListAsync();
            foreach (var instructor in instructors)
            {
                _instructorRepository.Delete(instructor);
            }
            await _instructorRepository.SaveChangesAsync();

            var courses = await _courseRepository.Find(c => c.DepartmentId == id).ToListAsync();
            foreach (var course in courses)
            {
                _courseRepository.Delete(course);
            }
            await _courseRepository.SaveChangesAsync();

            _repository.Delete(department);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> DepartmentExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null)
        {
            var existing = await _repository.GetByCodeAsync(code);
            if (existing == null) return true;
            if (excludeId.HasValue && existing.Id == excludeId.Value) return true;
            return false;
        }
    }
}