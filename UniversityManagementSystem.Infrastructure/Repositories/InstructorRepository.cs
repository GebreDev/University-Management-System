using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class InstructorRepository : GenericRepository<Instructor>, IInstructorRepository, IRepository<Instructor>
{
	public InstructorRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<Instructor?> GetInstructorWithDetailsAsync(int id)
	{
		return await _context.Instructors.Include((Instructor t) => t.Department).Include((Instructor t) => t.ApplicationUser).Include((Instructor t) => t.Courses)
			.FirstOrDefaultAsync((Instructor t) => t.Id == id);
	}

	public async Task<IEnumerable<Instructor>> GetAllInstructorsWithDetailsAsync()
	{
		return await _context.Instructors.Include((Instructor t) => t.Department).Include((Instructor t) => t.ApplicationUser).Include((Instructor t) => t.Courses)
			.ToListAsync();
	}

	public async Task<Instructor?> GetByApplicationUserIdAsync(string userId)
	{
		return await _context.Instructors.Include((Instructor t) => t.ApplicationUser).FirstOrDefaultAsync((Instructor t) => t.ApplicationUserId == userId);
	}
}
