using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class GradeRepository : GenericRepository<Grade>, IGradeRepository, IRepository<Grade>
{
	public GradeRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId)
	{
		return await _dbSet.Where((Grade g) => g.StudentId == studentId).ToListAsync();
	}

	public async Task<IEnumerable<Grade>> GetByCourseIdAsync(int courseId)
	{
		return await _dbSet.Where((Grade g) => g.CourseId == courseId).ToListAsync();
	}

	public async Task<IEnumerable<Grade>> GetByStudentAndCourseAsync(int studentId, int courseId)
	{
		return await _dbSet.Where((Grade g) => g.StudentId == studentId && g.CourseId == courseId).ToListAsync();
	}

	public async Task<IEnumerable<Grade>> GetByExamIdAsync(int examId)
	{
		return await _dbSet.Where((Grade g) => g.ExamId == (int?)examId).ToListAsync();
	}

	public async Task<IEnumerable<Grade>> GetByAssignmentIdAsync(int assignmentId)
	{
		return await _dbSet.Where((Grade g) => g.AssignmentId == (int?)assignmentId).ToListAsync();
	}

	public async Task<IEnumerable<Grade>> GetByCourseIdsAsync(IEnumerable<int> courseIds)
	{
		return await _dbSet.Where((Grade g) => courseIds.Contains(g.CourseId)).ToListAsync();
	}
}
