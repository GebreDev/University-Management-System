using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class AssignmentAnswerRepository : GenericRepository<AssignmentAnswer>, IAssignmentAnswerRepository, IRepository<AssignmentAnswer>
{
	public AssignmentAnswerRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<AssignmentAnswer>> GetByStudentAndAssignmentAsync(int studentId, int assignmentId)
	{
		return await _dbSet.Where((AssignmentAnswer aa) => aa.StudentId == studentId && aa.AssignmentQuestion.AssignmentId == assignmentId).Include((AssignmentAnswer aa) => aa.AssignmentQuestion).ToListAsync();
	}

	public async Task<IEnumerable<AssignmentAnswer>> GetByAssignmentQuestionIdAsync(int questionId)
	{
		return await _dbSet.Where((AssignmentAnswer aa) => aa.AssignmentQuestionId == questionId).ToListAsync();
	}
}
