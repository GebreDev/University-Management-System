using UniversityManagementSystem.Application.DTOs.Common;
using UniversityManagementSystem.Application.DTOs.UserManagement;
using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<bool> IsInRoleAsync(string role);
        Task<string?> GetUserRoleAsync();
        Task<Student?> GetCurrentStudentAsync();
        Task<Instructor?> GetCurrentInstructorAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task UpdateProfileAsync(UserProfileDto model);
        Task UpdateProfilePictureAsync(string userId, string pictureUrl);
        Task<PagedResult<UserListDto>> GetPagedUsersAsync(string? role, string? search, int page, int pageSize);
        Task<UserUpdateDto?> GetUserForEditAsync(string userId);
        Task CreateUserAsync(UserCreateDto dto);
        Task UpdateUserAsync(UserUpdateDto dto);
        Task ActivateUserAsync(string userId);
        Task DeactivateUserAsync(string userId);
    }
}