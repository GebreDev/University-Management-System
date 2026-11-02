using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository, IRepository<AuditLog>
{
	public AuditLogRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public IQueryable<AuditLog> Query()
	{
		return _dbSet.AsNoTracking();
	}

	public new async Task<IEnumerable<AuditLog>> GetAllAsync()
	{
		return await _dbSet.AsNoTracking().ToListAsync();
	}
}
