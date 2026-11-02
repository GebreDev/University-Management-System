using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class AssignmentQuestionRepository : GenericRepository<AssignmentQuestion>, IAssignmentQuestionRepository, IRepository<AssignmentQuestion>
{
	public AssignmentQuestionRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<AssignmentQuestion>> GetByAssignmentIdAsync(int assignmentId)
	{
		return await _dbSet.Where((AssignmentQuestion q) => q.AssignmentId == assignmentId).Include((AssignmentQuestion q) => q.Options).ToListAsync();
	}

	public async Task<AssignmentQuestion?> GetByIdWithOptionsAsync(int questionId)
	{
		return await _dbSet.Include((AssignmentQuestion q) => q.Options).FirstOrDefaultAsync((AssignmentQuestion q) => q.Id == questionId);
	}
}
