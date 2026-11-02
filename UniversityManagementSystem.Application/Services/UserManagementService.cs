using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Common;
using UniversityManagementSystem.Application.DTOs.UserManagement;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStudentRepository _studentRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IStudentRepository studentRepo,
            IInstructorRepository instructorRepo,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IAuditLogService auditLogService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _studentRepo = studentRepo;
            _instructorRepo = instructorRepo;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var context = _httpContextAccessor.HttpContext
                ?? throw new UnauthorizedAccessException("No HTTP context available.");
            return await _userManager.GetUserAsync(context.User)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            return await _userManager.IsInRoleAsync(await GetCurrentUserAsync(), role);
        }

        public async Task<string?> GetUserRoleAsync()
        {
            var roles = await _userManager.GetRolesAsync(await GetCurrentUserAsync());
            return roles.FirstOrDefault();
        }

        public async Task<Student?> GetCurrentStudentAsync()
        {
            return await _studentRepo.GetByApplicationUserIdAsync((await GetCurrentUserAsync()).Id);
        }

        public async Task<Instructor?> GetCurrentInstructorAsync()
        {
            return await _instructorRepo.GetByApplicationUserIdAsync((await GetCurrentUserAsync()).Id);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl ?? "",
                Role = roles.FirstOrDefault() ?? "",
                CreatedAt = user.CreatedAt
            };
        }

        public async Task UpdateProfileAsync(UserProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) throw new Exception("User not found.");
            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName?.Trim() ?? string.Empty;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth ?? user.DateOfBirth;
            await _userManager.UpdateAsync(user);

            var student = await _studentRepo.GetByApplicationUserIdAsync(user.Id);
            if (student != null)
            {
                student.FirstName = user.FirstName;
                student.MiddleName = user.MiddleName;
                student.LastName = user.LastName;
                await _studentRepo.UpdateAsync(student);
                await _studentRepo.SaveChangesAsync();
            }

            var instructor = await _instructorRepo.GetByApplicationUserIdAsync(user.Id);
            if (instructor != null)
            {
                instructor.FirstName = user.FirstName;
                instructor.MiddleName = user.MiddleName;
                instructor.LastName = user.LastName;
                await _instructorRepo.UpdateAsync(instructor);
                await _instructorRepo.SaveChangesAsync();
            }

            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "User", "Update", "Profile", null, "Updated user profile");
        }

        public async Task UpdateProfilePictureAsync(string userId, string pictureUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");
            user.ProfilePictureUrl = pictureUrl;
            await _userManager.UpdateAsync(user);
            await _auditLogService.LogAsync(userId, user.FirstName + " " + user.LastName, "User", "Update", "Profile", null, "Updated profile picture");
        }

        public async Task<PagedResult<UserListDto>> GetPagedUsersAsync(string? role, string? search, int page, int pageSize)
        {
            var query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var userIds = usersInRole.Select(u => u.Id);
                query = query.Where(u => userIds.Contains(u.Id));
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => (u.Email ?? "").Contains(search) || (u.FirstName ?? "").Contains(search) || (u.LastName ?? "").Contains(search));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var userDtos = new List<UserListDto>();
            foreach (var user in items)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserListDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "",
                    IsActive = user.AccountStatus == Domain.Enums.AccountStatus.Active,
                    CreatedAt = user.CreatedAt
                });
            }
            return new PagedResult<UserListDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = userDtos
            };
        }

        public async Task<UserUpdateDto?> GetUserForEditAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return new UserUpdateDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                DateOfBirth = user.DateOfBirth,
                IsActive = user.AccountStatus == Domain.Enums.AccountStatus.Active
            };
        }

        public async Task CreateUserAsync(UserCreateDto dto)
        {
            var username = !string.IsNullOrEmpty(dto.Username) ? dto.Username : dto.Email;

            var user = new ApplicationUser
            {
                UserName = username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth ?? DateTime.UtcNow,
                Address = dto.Address ?? "",
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                AccountStatus = Domain.Enums.AccountStatus.Active
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            if (!string.IsNullOrEmpty(dto.Role))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    throw new Exception($"Failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", dto.Role ?? "User", "Create", "User", null, $"Created user account for {dto.Email}");
        }

        public async Task UpdateUserAsync(UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) throw new Exception("User not found.");
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
            user.AccountStatus = dto.IsActive ? Domain.Enums.AccountStatus.Active : Domain.Enums.AccountStatus.Deactivated;
            await _userManager.UpdateAsync(user);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Update", "User", null, $"Updated user account");
        }

        public async Task ActivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");
            user.AccountStatus = Domain.Enums.AccountStatus.Active;
            await _userManager.UpdateAsync(user);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Activate", "User", null, "Activated user account");
        }

        public async Task DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");
            user.AccountStatus = Domain.Enums.AccountStatus.Deactivated;
            await _userManager.UpdateAsync(user);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Deactivate", "User", null, "Deactivated user account");
        }
    }
}