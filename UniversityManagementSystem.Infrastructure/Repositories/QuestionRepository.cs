using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository, IRepository<Question>
{
	public QuestionRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<Question>> GetByExamIdAsync(int examId)
	{
		return await _dbSet.Where((Question q) => q.ExamId == examId).Include((Question q) => q.Options).ToListAsync();
	}

	public async Task<Question?> GetByIdWithOptionsAsync(int questionId)
	{
		return await _dbSet.Include((Question q) => q.Options).FirstOrDefaultAsync((Question q) => q.Id == questionId);
	}
}
