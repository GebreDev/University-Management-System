using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class SemesterOfferedCourseRepository : GenericRepository<SemesterOfferedCourse>, ISemesterOfferedCourseRepository, IRepository<SemesterOfferedCourse>
{
	public SemesterOfferedCourseRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<SemesterOfferedCourse>> GetOpenBySemesterIdAsync(int semesterId)
	{
		return await _dbSet.Where((SemesterOfferedCourse o) => o.SemesterId == semesterId && o.IsOpen).Include((SemesterOfferedCourse o) => o.Course).Include((SemesterOfferedCourse o) => o.Department)
			.ToListAsync();
	}

	public async Task<IEnumerable<SemesterOfferedCourse>> GetBySemesterIdAsync(int semesterId)
	{
		return await _dbSet.Where((SemesterOfferedCourse o) => o.SemesterId == semesterId).Include((SemesterOfferedCourse o) => o.Course).Include((SemesterOfferedCourse o) => o.Department)
			.ToListAsync();
	}

	public async Task<SemesterOfferedCourse?> GetByIdWithDetailsAsync(int id)
	{
		return await _dbSet.Include((SemesterOfferedCourse o) => o.Course).Include((SemesterOfferedCourse o) => o.Department).Include((SemesterOfferedCourse o) => o.Semester)
			.FirstOrDefaultAsync((SemesterOfferedCourse o) => o.Id == id);
	}

	public async Task<int> GetRegisteredCountAsync(int offeredCourseId)
	{
		return await _context.RegistrationItems.CountAsync((RegistrationItem i) => i.SemesterOfferedCourseId == offeredCourseId && (int)i.Status == 2);
	}

	public async Task<bool> IsStudentRegisteredAsync(int studentId, int offeredCourseId)
	{
		return await _context.RegistrationItems.AnyAsync((RegistrationItem i) => i.SemesterOfferedCourseId == offeredCourseId && i.Registration != null && i.Registration.StudentId == studentId && (int)i.Registration.Status == 3 && (int)i.Status == 2);
	}

	public async Task<IEnumerable<OfferedCourseSchedule>> GetSchedulesAsync(int offeredCourseId)
	{
		return await _context.OfferedCourseSchedules.Where((OfferedCourseSchedule s) => s.SemesterOfferedCourseId == offeredCourseId).ToListAsync();
	}
}
