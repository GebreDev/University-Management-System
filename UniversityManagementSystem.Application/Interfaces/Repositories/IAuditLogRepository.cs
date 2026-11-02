using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        new Task<IEnumerable<AuditLog>> GetAllAsync();

        /// <summary>
        /// Composable read-only query for DB-side filtering and paging.
        /// </summary>
        IQueryable<AuditLog> Query();
    }
}