using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository, IRepository<Assignment>
{
	public AssignmentRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<Assignment>> GetByCourseIdAsync(int courseId)
	{
		return await _dbSet.Where((Assignment a) => a.CourseId == courseId).Include((Assignment a) => a.Course).Include((Assignment a) => a.Instructor)
			.Include((Assignment a) => a.Submissions)
			.Include((Assignment a) => a.AssignmentQuestions)
			.ToListAsync();
	}

	public async Task<IEnumerable<Assignment>> GetByCourseIdsAsync(IEnumerable<int> courseIds)
	{
		List<int> ids = courseIds.ToList();
		if (!ids.Any())
		{
			return new List<Assignment>();
		}
		return await _dbSet.Where((Assignment a) => ids.Contains(a.CourseId)).Include((Assignment a) => a.Course).Include((Assignment a) => a.Instructor)
			.Include((Assignment a) => a.Submissions)
			.Include((Assignment a) => a.AssignmentQuestions)
			.ToListAsync();
	}

	public async Task<IEnumerable<Assignment>> GetPublishedAsync()
	{
		return await _dbSet.Where((Assignment a) => a.IsPublished).Include((Assignment a) => a.Course).Include((Assignment a) => a.Instructor)
			.Include((Assignment a) => a.Submissions)
			.Include((Assignment a) => a.AssignmentQuestions)
			.ToListAsync();
	}

	public async Task<IEnumerable<Assignment>> GetByInstructorIdAsync(int instructorId)
	{
		return await _dbSet.Where((Assignment a) => a.InstructorId == instructorId).Include((Assignment a) => a.Course).Include((Assignment a) => a.Submissions)
			.Include((Assignment a) => a.AssignmentQuestions)
			.ToListAsync();
	}
}
