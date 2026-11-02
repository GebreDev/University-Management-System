using UniversityManagementSystem.Application.DTOs.Modules;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IModuleService
    {
        /// <summary>
        /// Builds a data payload (real stats + items) for a module overview page,
        /// tailored to the current user's role.
        /// </summary>
        Task<ModuleOverviewDto?> GetModuleOverviewAsync(string moduleKey, string role, string userId);
    }
}
