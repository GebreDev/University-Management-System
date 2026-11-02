using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository, IRepository<Enrollment>
{
	public EnrollmentRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<Enrollment?> GetEnrollmentWithDetailsAsync(int id)
	{
		return await _context.Enrollments.Include((Enrollment e) => e.Student).ThenInclude((Student s) => s.Department).Include((Enrollment e) => e.Course)
			.ThenInclude((Course c) => c.Instructor)
			.FirstOrDefaultAsync((Enrollment e) => e.Id == id);
	}

	public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsWithDetailsAsync()
	{
		return await _context.Enrollments.Include((Enrollment e) => e.Student).Include((Enrollment e) => e.Course).ToListAsync();
	}

	public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId)
	{
		return await _context.Enrollments.Where((Enrollment e) => e.StudentId == studentId).Include((Enrollment e) => e.Course).ToListAsync();
	}

	public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId)
	{
		return await _context.Enrollments.Where((Enrollment e) => e.CourseId == courseId).Include((Enrollment e) => e.Student).ToListAsync();
	}

	public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdsAsync(IEnumerable<int> courseIds)
	{
		List<int> ids = courseIds.ToList();
		if (!ids.Any())
		{
			return new List<Enrollment>();
		}
		return await _context.Enrollments.Where((Enrollment e) => ids.Contains(e.CourseId)).Include((Enrollment e) => e.Student).Include((Enrollment e) => e.Course)
			.ToListAsync();
	}

	public async Task<IEnumerable<Enrollment>> GetPendingEnrollmentsAsync()
	{
		return await _context.Enrollments.Where((Enrollment e) => (int)e.Status == 1).Include((Enrollment e) => e.Student).Include((Enrollment e) => e.Course)
			.ToListAsync();
	}

	public async Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId)
	{
		return await _context.Enrollments.AnyAsync((Enrollment e) => e.StudentId == studentId && e.CourseId == courseId && (int)e.Status != 3);
	}
}
