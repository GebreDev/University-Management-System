using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class RegistrationRepository : GenericRepository<Registration>, IRegistrationRepository, IRepository<Registration>
{
	public RegistrationRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<Registration?> GetByIdWithDetailsAsync(int id)
	{
		return await _dbSet.Include((Registration r) => r.Student).ThenInclude((Student s) => s.Department).Include((Registration r) => r.Semester)
			.Include((Registration r) => r.Department)
			.Include((Registration r) => r.Items)
			.ThenInclude((RegistrationItem i) => i.SemesterOfferedCourse)
			.ThenInclude((SemesterOfferedCourse o) => o.Course)
			.Include((Registration r) => r.StatusHistory)
			.FirstOrDefaultAsync((Registration r) => r.Id == id);
	}

	public async Task<IEnumerable<Registration>> GetByStudentIdAsync(int studentId)
	{
		return await (from r in _dbSet.Where((Registration r) => r.StudentId == studentId).Include((Registration r) => r.Semester).Include((Registration r) => r.Items)
			orderby r.CreatedAt descending
			select r).ToListAsync();
	}

	public async Task<IEnumerable<Registration>> GetBySemesterIdAsync(int semesterId)
	{
		return await (from r in _dbSet.Where((Registration r) => r.SemesterId == semesterId).Include((Registration r) => r.Student).ThenInclude((Student s) => s.Department)
				.Include((Registration r) => r.Items)
			orderby r.CreatedAt descending
			select r).ToListAsync();
	}

	public async Task<IEnumerable<Registration>> GetPendingBySemesterIdAsync(int semesterId)
	{
		return await (from r in _dbSet.Where((Registration r) => r.SemesterId == semesterId && (int)r.Status == 2).Include((Registration r) => r.Student).Include((Registration r) => r.Items)
				.Include((Registration r) => r.Department)
			orderby r.CreatedAt descending
			select r).ToListAsync();
	}

	public async Task<int> GetApprovedCourseCountAsync(int studentId, int semesterId)
	{
		return await _context.RegistrationItems.Where((RegistrationItem i) => i.Registration.StudentId == studentId && i.Registration.SemesterId == semesterId && (int)i.Registration.Status == 3 && (int)i.Status == 2).CountAsync();
	}

	public async Task AddStatusHistoryAsync(RegistrationStatusHistory history)
	{
		await _context.AddAsync(history);
	}

	public async Task<Registration?> GetByStudentAndSemesterAsync(int studentId, int semesterId)
	{
		return await _dbSet.Include((Registration r) => r.Items).FirstOrDefaultAsync((Registration r) => r.StudentId == studentId && r.SemesterId == semesterId);
	}
}
