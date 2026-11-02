using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniversityManagementSystem.Web.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string Permission { get; }

        public PermissionAttribute(string permission)
        {
            Permission = permission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
            if (permissionService == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = await permissionService.HasPermissionAsync(context.HttpContext.User, Permission);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}