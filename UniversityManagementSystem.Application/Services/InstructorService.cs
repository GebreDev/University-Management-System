using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Instructor;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly IInstructorRepository _repository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public InstructorService(IInstructorRepository repository, IDepartmentRepository departmentRepository,
            UserManager<ApplicationUser> userManager, IMapper mapper, IAuditLogService auditLogService)
        {
            _repository = repository;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<InstructorListDto>> GetAllInstructorsAsync()
        {
            var instructors = await _repository.GetAllInstructorsWithDetailsAsync();
            return _mapper.Map<IEnumerable<InstructorListDto>>(instructors);
        }

        public async Task<InstructorViewDto> GetInstructorByIdAsync(int id)
        {
            var instructor = await _repository.GetInstructorWithDetailsAsync(id);
            if (instructor == null)
                throw new NotFoundException(nameof(Instructor), id);
            return _mapper.Map<InstructorViewDto>(instructor);
        }

        public async Task<InstructorViewDto> CreateInstructorAsync(InstructorCreateDto dto)
        {
            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new NotFoundException(nameof(Department), dto.DepartmentId);

            var username = !string.IsNullOrEmpty(dto.Username) ? dto.Username : dto.Email;

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

            await _userManager.AddToRoleAsync(user, "Instructor");

            var instructor = _mapper.Map<Instructor>(dto);
            instructor.ApplicationUserId = user.Id;
            await _repository.AddAsync(instructor);
            await _repository.SaveChangesAsync();

            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "Create", "Instructor", null, $"Created instructor account for {dto.FirstName} {dto.LastName}");

            return _mapper.Map<InstructorViewDto>(instructor);
        }

        public async Task UpdateInstructorAsync(InstructorUpdateDto dto)
        {
            var instructor = await _repository.GetInstructorWithDetailsAsync(dto.Id);
            if (instructor == null)
                throw new NotFoundException(nameof(Instructor), dto.Id);

            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new NotFoundException(nameof(Department), dto.DepartmentId);

            var user = await _userManager.FindByIdAsync(instructor.ApplicationUserId);
            if (user != null)
            {
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.UserName = dto.Email;
                await _userManager.UpdateAsync(user);
            }

            _mapper.Map(dto, instructor);
            await _repository.UpdateAsync(instructor);
            await _repository.SaveChangesAsync();

            await _auditLogService.LogAsync(instructor.ApplicationUserId, $"{dto.FirstName} {dto.LastName}", "Instructor", "Update", "Instructor", dto.Id, $"Updated instructor ID {dto.Id}");
        }

        public async Task DeleteInstructorAsync(int id)
        {
            var instructor = await _repository.GetByIdAsync(id);
            if (instructor == null)
                throw new NotFoundException(nameof(Instructor), id);

            var user = await _userManager.FindByIdAsync(instructor.ApplicationUserId);
            if (user != null)
            {
                user.AccountStatus = Domain.Enums.AccountStatus.Deactivated;
                await _userManager.UpdateAsync(user);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "Delete", "Instructor", id, $"Deleted instructor ID {id}");
            }

            _repository.Delete(instructor);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> InstructorExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<InstructorViewDto?> GetInstructorByUserIdAsync(string userId)
        {
            var instructor = await _repository.GetByApplicationUserIdAsync(userId);
            if (instructor == null)
                return null;
            return _mapper.Map<InstructorViewDto>(instructor);
        }
    }
}