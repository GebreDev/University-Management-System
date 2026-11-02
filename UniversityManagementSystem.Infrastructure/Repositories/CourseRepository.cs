using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository, IRepository<Course>
{
	public CourseRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<Course?> GetCourseWithDetailsAsync(int id)
	{
		return await _dbSet.Include((Course c) => c.Department).Include((Course c) => c.Instructor).Include((Course c) => c.Enrollments)
			.ThenInclude((Enrollment e) => e.Student)
			.FirstOrDefaultAsync((Course c) => c.Id == id);
	}

	public async Task<IEnumerable<Course>> GetAllCoursesWithDetailsAsync()
	{
		return await _dbSet.Include((Course c) => c.Department).Include((Course c) => c.Instructor).Include((Course c) => c.Enrollments)
			.ThenInclude((Enrollment e) => e.Student)
			.ToListAsync();
	}

	public async Task<IEnumerable<Course>> GetCoursesByInstructorIdAsync(int instructorId)
	{
		return await _dbSet.Where((Course c) => c.InstructorId == instructorId).Include((Course c) => c.Department).Include((Course c) => c.Instructor)
			.Include((Course c) => c.Enrollments)
			.ToListAsync();
	}

	public async Task<IEnumerable<Course>> GetCoursesByDepartmentIdAsync(int departmentId)
	{
		return await _dbSet.Where((Course c) => c.DepartmentId == departmentId).ToListAsync();
	}

	public async Task<Course?> GetByCodeAsync(string code)
	{
		return await _dbSet.FirstOrDefaultAsync((Course c) => c.Code == code);
	}

	public async Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId)
	{
		return await _dbSet.Where((Course c) => c.InstructorId == instructorId).ToListAsync();
	}
}
