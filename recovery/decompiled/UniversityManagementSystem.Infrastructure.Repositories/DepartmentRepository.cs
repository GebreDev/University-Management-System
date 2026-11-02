using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository, IRepository<Department>
{
	public DepartmentRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<Department?> GetDepartmentWithDetailsAsync(int id)
	{
		return await _context.Departments.Include((Department d) => d.Students).Include((Department d) => d.Instructors).Include((Department d) => d.Courses)
			.FirstOrDefaultAsync((Department d) => d.Id == id);
	}

	public async Task<IEnumerable<Department>> GetAllDepartmentsWithDetailsAsync()
	{
		return await _context.Departments.Include((Department d) => d.Students).Include((Department d) => d.Instructors).Include((Department d) => d.Courses)
			.ToListAsync();
	}

	public async Task<Department?> GetByCodeAsync(string code)
	{
		return await _context.Departments.FirstOrDefaultAsync((Department d) => d.Code == code);
	}
}
