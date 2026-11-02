using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class CourseResultRepository : GenericRepository<CourseResult>, ICourseResultRepository, IRepository<CourseResult>
{
	public CourseResultRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<CourseResult>> GetByStudentIdAsync(int studentId)
	{
		return await _dbSet.Where((CourseResult r) => r.StudentId == studentId).ToListAsync();
	}

	public async Task<IEnumerable<CourseResult>> GetPublishedByStudentIdAsync(int studentId)
	{
		return await _dbSet.Where((CourseResult r) => r.StudentId == studentId && r.IsPublished).ToListAsync();
	}

	public async Task<IEnumerable<CourseResult>> GetByCourseIdAsync(int courseId)
	{
		return await _dbSet.Where((CourseResult r) => r.CourseId == courseId).ToListAsync();
	}

	public async Task<IEnumerable<CourseResult>> GetByCourseIdsAsync(IEnumerable<int> courseIds)
	{
		List<int> ids = courseIds.ToList();
		if (!ids.Any())
		{
			return new List<CourseResult>();
		}
		return await _dbSet.Where((CourseResult r) => ids.Contains(r.CourseId)).ToListAsync();
	}

	public async Task<CourseResult?> GetByStudentAndCourseAsync(int studentId, int courseId)
	{
		return await _dbSet.FirstOrDefaultAsync((CourseResult r) => r.StudentId == studentId && r.CourseId == courseId);
	}
}
