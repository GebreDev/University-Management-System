using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class ExamResultRepository : GenericRepository<ExamResult>, IExamResultRepository, IRepository<ExamResult>
{
	public ExamResultRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<ExamResult>> GetByExamIdAsync(int examId)
	{
		return await _dbSet.Where((ExamResult er) => er.ExamId == examId).Include((ExamResult er) => er.Student).ToListAsync();
	}

	public async Task<ExamResult?> GetByStudentAndExamAsync(int studentId, int examId)
	{
		return await _dbSet.FirstOrDefaultAsync((ExamResult er) => er.StudentId == studentId && er.ExamId == examId);
	}

	public async Task<IEnumerable<ExamResult>> GetByStudentIdAsync(int studentId)
	{
		return await _dbSet.Where((ExamResult er) => er.StudentId == studentId).Include((ExamResult er) => er.Exam).ToListAsync();
	}
}
