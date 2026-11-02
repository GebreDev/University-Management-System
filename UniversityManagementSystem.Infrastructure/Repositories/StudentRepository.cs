using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository, IRepository<Student>
{
	public StudentRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<Student?> GetStudentWithDetailsAsync(int id)
	{
		return await _dbSet.Include((Student s) => s.Department).Include((Student s) => s.ApplicationUser).Include((Student s) => s.Enrollments)
			.ThenInclude((Enrollment e) => e.Course)
			.FirstOrDefaultAsync((Student s) => s.Id == id);
	}

	public async Task<IEnumerable<Student>> GetAllStudentsWithDetailsAsync()
	{
		return await _dbSet.Include((Student s) => s.Department).Include((Student s) => s.ApplicationUser).Include((Student s) => s.Enrollments)
			.ThenInclude((Enrollment e) => e.Course)
			.ToListAsync();
	}

	public async Task<Student?> GetByStudentIdAsync(string studentId)
	{
		return await _dbSet.FirstOrDefaultAsync((Student s) => s.StudentId == studentId);
	}

	public async Task<Student?> GetByApplicationUserIdAsync(string userId)
	{
		return await _dbSet.Include((Student s) => s.ApplicationUser).FirstOrDefaultAsync((Student s) => s.ApplicationUserId == userId);
	}

	public async Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId)
	{
		return await _dbSet.Where((Student s) => s.Enrollments.Any((Enrollment e) => e.CourseId == courseId)).Include((Student s) => s.Department).Include((Student s) => s.ApplicationUser)
			.Include((Student s) => s.Enrollments)
			.ToListAsync();
	}

	public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId)
	{
		return await _context.Enrollments.Where((Enrollment e) => e.StudentId == studentId).Include((Enrollment e) => e.Course).ToListAsync();
	}

	public async Task<IEnumerable<Student>> GetBySectionAsync(int sectionId)
	{
		return await (from s in _dbSet.Include((Student s) => s.Department).Include((Student s) => s.ApplicationUser)
			where s.AcademicSectionId == (int?)sectionId
			select s).ToListAsync();
	}

	public async Task<Student?> GetStudentWithUserAsync(int id)
	{
		return await _dbSet.Include((Student s) => s.ApplicationUser).FirstOrDefaultAsync((Student s) => s.Id == id);
	}

	public async Task<Semester?> GetSemesterWithYearAsync(int semesterId)
	{
		return await _context.Semesters.Include((Semester s) => s.AcademicYear).FirstOrDefaultAsync((Semester s) => s.Id == semesterId);
	}

	public async Task<FinanceSetting> GetFinanceSettingsAsync()
	{
		FinanceSetting settings = await _context.FinanceSettings.FirstOrDefaultAsync();
		if (settings == null)
		{
			settings = new FinanceSetting();
			_context.FinanceSettings.Add(settings);
			await _context.SaveChangesAsync();
		}
		return settings;
	}
}
