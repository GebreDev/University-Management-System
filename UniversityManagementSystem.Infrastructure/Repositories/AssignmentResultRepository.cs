using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class AssignmentResultRepository : GenericRepository<AssignmentResult>, IAssignmentResultRepository, IRepository<AssignmentResult>
{
	public AssignmentResultRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<AssignmentResult>> GetByAssignmentIdAsync(int assignmentId)
	{
		return await _dbSet.Where((AssignmentResult r) => r.AssignmentId == assignmentId).Include((AssignmentResult r) => r.Student).Include((AssignmentResult r) => r.Assignment)
			.ToListAsync();
	}

	public async Task<AssignmentResult?> GetByStudentAndAssignmentAsync(int studentId, int assignmentId)
	{
		return await _dbSet.FirstOrDefaultAsync((AssignmentResult r) => r.StudentId == studentId && r.AssignmentId == assignmentId);
	}

	public async Task<IEnumerable<AssignmentResult>> GetByStudentIdAsync(int studentId)
	{
		return await _dbSet.Where((AssignmentResult r) => r.StudentId == studentId).Include((AssignmentResult r) => r.Assignment).ToListAsync();
	}
}
