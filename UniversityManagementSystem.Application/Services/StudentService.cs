using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Student;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public StudentService(IStudentRepository repository, IDepartmentRepository departmentRepository,
            UserManager<ApplicationUser> userManager, IMapper mapper, IAuditLogService auditLogService)
        {
            _repository = repository;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<StudentListDto>> GetAllStudentsAsync()
        {
            var students = await _repository.GetAllStudentsWithDetailsAsync();
            return _mapper.Map<IEnumerable<StudentListDto>>(students);
        }

        public async Task<StudentViewDto> GetStudentByIdAsync(int id)
        {
            var student = await _repository.GetStudentWithDetailsAsync(id);
            if (student == null)
                throw new NotFoundException(nameof(Student), id);
            return _mapper.Map<StudentViewDto>(student);
        }

        public async Task<StudentViewDto> CreateStudentAsync(StudentCreateDto dto)
        {
            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new NotFoundException(nameof(Department), dto.DepartmentId);

            var existingStudent = await _repository.GetByStudentIdAsync(dto.StudentId);
            if (existingStudent != null)
                throw new DuplicateException($"Student with ID '{dto.StudentId}' already exists.");

            // Student login username is the StudentId (UGR).
            var username = !string.IsNullOrEmpty(dto.Username) ? dto.Username : dto.StudentId;

            var user = new ApplicationUser
            {
                UserName = username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = System.DateTime.UtcNow,
                Address = ""
            };
            var createUserResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createUserResult.Succeeded)
                throw new CustomException("Failed to create user account.");

            await _userManager.AddToRoleAsync(user, "Student");

            var student = _mapper.Map<Student>(dto);
            student.ApplicationUserId = user.Id;
            await _repository.AddAsync(student);
            await _repository.SaveChangesAsync();

            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Student", "Create", "Student", null, $"Created student account for {dto.FirstName} {dto.LastName}");

            return _mapper.Map<StudentViewDto>(student);
        }

        public async Task UpdateStudentAsync(StudentUpdateDto dto)
        {
            var student = await _repository.GetStudentWithDetailsAsync(dto.Id);
            if (student == null)
                throw new NotFoundException(nameof(Student), dto.Id);

            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new NotFoundException(nameof(Department), dto.DepartmentId);

            var user = await _userManager.FindByIdAsync(student.ApplicationUserId);
            if (user != null)
            {
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.UserName = dto.Email;
                await _userManager.UpdateAsync(user);
            }

            _mapper.Map(dto, student);
            await _repository.UpdateAsync(student);
            await _repository.SaveChangesAsync();

            await _auditLogService.LogAsync(student.ApplicationUserId, $"{dto.FirstName} {dto.LastName}", "Student", "Update", "Student", dto.Id, $"Updated student ID {dto.Id}");
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
                throw new NotFoundException(nameof(Student), id);

            var user = await _userManager.FindByIdAsync(student.ApplicationUserId);
            if (user != null)
            {
                user.AccountStatus = Domain.Enums.AccountStatus.Deactivated;
                await _userManager.UpdateAsync(user);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Student", "Delete", "Student", id, $"Deleted student ID {id}");
            }

            _repository.Delete(student);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<StudentViewDto?> GetStudentByUserIdAsync(string userId)
        {
            var student = await _repository.GetByApplicationUserIdAsync(userId);
            if (student == null)
                return null;
            return _mapper.Map<StudentViewDto>(student);
        }

        public async Task<IEnumerable<StudentListDto>> GetStudentsByCourseAsync(int courseId)
        {
            var students = await _repository.GetStudentsByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<StudentListDto>>(students);
        }
    }
}