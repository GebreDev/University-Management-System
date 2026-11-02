using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class ExamRepository : GenericRepository<Exam>, IExamRepository, IRepository<Exam>
{
	public ExamRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<Exam>> GetByCourseIdAsync(int courseId)
	{
		return await _dbSet.Where((Exam e) => e.CourseId == courseId).Include((Exam e) => e.Course).ToListAsync();
	}

	public async Task<Exam?> GetWithQuestionsAsync(int examId)
	{
		return await _dbSet.Include((Exam e) => e.Questions).ThenInclude((Question q) => q.Options).FirstOrDefaultAsync((Exam e) => e.Id == examId);
	}

	public async Task<IEnumerable<ExamResult>> GetResultsByExamIdAsync(int examId)
	{
		return await _context.ExamResults.Where((ExamResult er) => er.ExamId == examId).Include((ExamResult er) => er.Student).ToListAsync();
	}

	public async Task<IEnumerable<ExamResult>> GetResultsByExamIdsAsync(IEnumerable<int> examIds)
	{
		List<int> ids = examIds.ToList();
		if (!ids.Any())
		{
			return new List<ExamResult>();
		}
		return await _context.ExamResults.Where((ExamResult er) => ids.Contains(er.ExamId)).Include((ExamResult er) => er.Student).Include((ExamResult er) => er.Exam)
			.ToListAsync();
	}

	public async Task<ExamResult?> GetResultByStudentAndExamAsync(int studentId, int examId)
	{
		return await _context.ExamResults.FirstOrDefaultAsync((ExamResult er) => er.StudentId == studentId && er.ExamId == examId);
	}

	public async Task<IEnumerable<Exam>> GetPublishedExamsAsync()
	{
		return await _dbSet.Where((Exam e) => e.IsPublished).Include((Exam e) => e.Course).ToListAsync();
	}

	public async Task<IEnumerable<Exam>> GetByInstructorIdAsync(int instructorId)
	{
		return await _dbSet.Where((Exam e) => e.InstructorId == instructorId).Include((Exam e) => e.Course).ToListAsync();
	}

	public async Task<IEnumerable<Exam>> GetByDepartmentIdAsync(int departmentId)
	{
		return await _dbSet.Where((Exam e) => e.Course.DepartmentId == departmentId).ToListAsync();
	}
}
