using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class EventRepository : GenericRepository<UniversityEvent>, IEventRepository, IRepository<UniversityEvent>
{
	public EventRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<UniversityEvent>> GetUpcomingAsync(int count)
	{
		return await (from e in _dbSet
			where e.StartDateTime >= DateTime.UtcNow && e.IsPublic
			orderby e.StartDateTime
			select e).Take(count).ToListAsync();
	}

	public async Task<IEnumerable<UniversityEvent>> GetByDateRangeAsync(DateTime from, DateTime to)
	{
		return await (from e in _dbSet
			where e.StartDateTime >= @from && e.StartDateTime <= to
			orderby e.StartDateTime
			select e).ToListAsync();
	}
}
