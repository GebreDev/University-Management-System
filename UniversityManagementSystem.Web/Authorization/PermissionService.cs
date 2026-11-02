using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Web.Authorization
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PermissionService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(System.Security.Claims.ClaimsPrincipal user, string permission)
        {
            // Admins always have full access
            if (user.IsInRole("Admin"))
                return true;

            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return false;

            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null) return false;
            var roles = await _userManager.GetRolesAsync(appUser);

            if (!roles.Any())
                return false;

            var roleIds = await _context.Roles
                .Where(r => roles.Contains(r.Name!))
                .Select(r => r.Id)
                .ToListAsync();

            return await _context.RolePermissions
                .AnyAsync(rp => roleIds.Contains(rp.RoleId) && rp.Permission == permission);
        }
    }
}