using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class StudentAnswerRepository : GenericRepository<StudentAnswer>, IStudentAnswerRepository, IRepository<StudentAnswer>
{
	public StudentAnswerRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<StudentAnswer>> GetByStudentAndExamAsync(int studentId, int examId)
	{
		return await _dbSet.Where((StudentAnswer sa) => sa.StudentId == studentId && sa.Question.ExamId == examId).Include((StudentAnswer sa) => sa.Question).ToListAsync();
	}

	public async Task<IEnumerable<StudentAnswer>> GetByQuestionIdAsync(int questionId)
	{
		return await _dbSet.Where((StudentAnswer sa) => sa.QuestionId == questionId).ToListAsync();
	}
}
