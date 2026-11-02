using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class CourseScheduleRepository : GenericRepository<CourseSchedule>, ICourseScheduleRepository, IRepository<CourseSchedule>
{
	public CourseScheduleRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<CourseSchedule>> GetByCourseIdsAsync(IEnumerable<int> courseIds)
	{
		List<int> ids = courseIds.ToList();
		if (!ids.Any())
		{
			return new List<CourseSchedule>();
		}
		return await (from s in _dbSet.Include((CourseSchedule s) => s.Course)
			where ids.Contains(s.CourseId) && s.IsActive
			orderby s.DayOfWeek, s.StartTime
			select s).ToListAsync();
	}

	public async Task<IEnumerable<CourseSchedule>> GetByCourseIdAsync(int courseId)
	{
		return await (from s in _dbSet.Include((CourseSchedule s) => s.Course)
			where s.CourseId == courseId && s.IsActive
			orderby s.DayOfWeek, s.StartTime
			select s).ToListAsync();
	}
}
