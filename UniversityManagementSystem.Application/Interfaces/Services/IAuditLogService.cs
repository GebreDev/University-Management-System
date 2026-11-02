using UniversityManagementSystem.Application.DTOs.Audit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string? userId, string userName, string userRole, string action, string entity, int? entityId, string details);

        /// <summary>
        /// Backward-compatible full read with optional filters.
        /// </summary>
        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(string? userId = null, string? entity = null, string? action = null, string? searchText = null, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Paged, DB-side filtered read for the audit log screen.
        /// </summary>
        Task<(IEnumerable<AuditLogDto> Items, int TotalCount)> SearchAsync(
            string? userId = null, string? entity = null, string? action = null,
            string? searchText = null, DateTime? from = null, DateTime? to = null,
            int page = 1, int pageSize = 50);

        /// <summary>Distinct non-empty action names for the filter dropdown.</summary>
        Task<IEnumerable<string>> GetDistinctActionsAsync();

        /// <summary>Distinct non-empty entity types for the filter dropdown.</summary>
        Task<IEnumerable<string>> GetDistinctEntitiesAsync();

        Task<AuditLogDto> GetAuditLogByIdAsync(int id);
    }
}
