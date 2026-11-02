using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class CourseAssessmentRepository : GenericRepository<CourseAssessment>, ICourseAssessmentRepository, IRepository<CourseAssessment>
{
	public CourseAssessmentRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<CourseAssessment>> GetByCourseIdAsync(int courseId)
	{
		return await _dbSet.Where((CourseAssessment ca) => ca.CourseId == courseId).ToListAsync();
	}

	public async Task<IEnumerable<CourseAssessment>> GetByCourseIdWithMarksAsync(int courseId)
	{
		return await (from ca in _dbSet.Where((CourseAssessment ca) => ca.CourseId == courseId).Include((CourseAssessment ca) => ca.Marks)
			orderby ca.CreatedAt
			select ca).ToListAsync();
	}

	public async Task<IEnumerable<CourseAssessment>> GetByCourseIdsWithMarksAsync(IEnumerable<int> courseIds)
	{
		List<int> ids = courseIds.ToList();
		if (!ids.Any())
		{
			return new List<CourseAssessment>();
		}
		return await _dbSet.Where((CourseAssessment ca) => ids.Contains(ca.CourseId)).Include((CourseAssessment ca) => ca.Marks).ToListAsync();
	}

	public async Task DeleteByCourseIdAsync(int courseId)
	{
		List<CourseAssessment> assessments = await _dbSet.Where((CourseAssessment ca) => ca.CourseId == courseId).ToListAsync();
		if (assessments.Any())
		{
			_dbSet.RemoveRange(assessments);
		}
	}
}
