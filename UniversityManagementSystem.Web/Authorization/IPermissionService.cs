using System.Security.Claims;

namespace UniversityManagementSystem.Web.Authorization
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
    }
}