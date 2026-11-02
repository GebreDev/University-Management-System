using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository, IRepository<Submission>
{
	public SubmissionRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId)
	{
		return await _dbSet.Where((Submission s) => s.AssignmentId == assignmentId).Include((Submission s) => s.Student).ToListAsync();
	}

	public async Task<IEnumerable<Submission>> GetByAssignmentIdsAsync(IEnumerable<int> assignmentIds)
	{
		List<int> ids = assignmentIds.ToList();
		if (!ids.Any())
		{
			return new List<Submission>();
		}
		return await _dbSet.Where((Submission s) => ids.Contains(s.AssignmentId)).Include((Submission s) => s.Student).Include((Submission s) => s.Assignment)
			.ToListAsync();
	}

	public async Task<IEnumerable<Submission>> GetByStudentIdAsync(int studentId)
	{
		return await _dbSet.Where((Submission s) => s.StudentId == studentId).Include((Submission s) => s.Assignment).ToListAsync();
	}

	public async Task<Submission?> GetByStudentAndAssignmentAsync(int studentId, int assignmentId)
	{
		return await _dbSet.FirstOrDefaultAsync((Submission s) => s.StudentId == studentId && s.AssignmentId == assignmentId);
	}

	public async Task<IEnumerable<Submission>> GetGradedSubmissionsAsync(int assignmentId)
	{
		return await _dbSet.Where((Submission s) => s.AssignmentId == assignmentId && s.IsGraded).Include((Submission s) => s.Student).ToListAsync();
	}

	public async Task<IEnumerable<Submission>> GetUngradedSubmissionsAsync(int assignmentId)
	{
		return await _dbSet.Where((Submission s) => s.AssignmentId == assignmentId && !s.IsGraded).Include((Submission s) => s.Student).ToListAsync();
	}
}
