using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class SemesterRegistrationRepository : GenericRepository<SemesterRegistration>, ISemesterRegistrationRepository, IRepository<SemesterRegistration>
{
	public SemesterRegistrationRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public override async Task<SemesterRegistration?> GetByIdAsync(int id)
	{
		return await _dbSet.Include((SemesterRegistration r) => r.Student).Include((SemesterRegistration r) => r.AcademicProgram).ThenInclude((AcademicProgram p) => p.Department)
			.Include((SemesterRegistration r) => r.Semester)
			.ThenInclude((Semester s) => s.AcademicYear)
			.FirstOrDefaultAsync((SemesterRegistration r) => r.Id == id);
	}

	public new async Task<IEnumerable<SemesterRegistration>> GetAllAsync()
	{
		IOrderedQueryable<SemesterRegistration> query = from r in _dbSet.Include((SemesterRegistration r) => r.Student).Include((SemesterRegistration r) => r.AcademicProgram).ThenInclude((AcademicProgram p) => p.Department)
				.Include((SemesterRegistration r) => r.Semester)
			orderby r.CreatedAt descending
			select r;
		return await query.ToListAsync();
	}

	public async Task<IEnumerable<SemesterRegistration>> GetByStudentIdAsync(int studentId)
	{
		IOrderedQueryable<SemesterRegistration> query = from r in _dbSet.Where((SemesterRegistration r) => r.StudentId == studentId).Include((SemesterRegistration r) => r.AcademicProgram).Include((SemesterRegistration r) => r.Semester)
			orderby r.CreatedAt descending
			select r;
		return await query.ToListAsync();
	}

	public async Task<SemesterRegistration?> GetByStudentAndSemesterAsync(int studentId, int semesterId)
	{
		return await _dbSet.Include((SemesterRegistration r) => r.Student).Include((SemesterRegistration r) => r.AcademicProgram).Include((SemesterRegistration r) => r.Semester)
			.FirstOrDefaultAsync((SemesterRegistration r) => r.StudentId == studentId && r.SemesterId == (int?)semesterId);
	}

	public override async Task<SemesterRegistration> AddAsync(SemesterRegistration entity)
	{
		await _dbSet.AddAsync(entity);
		return entity;
	}

	public override async Task<SemesterRegistration> UpdateAsync(SemesterRegistration entity)
	{
		_dbSet.Update(entity);
		return entity;
	}

	public override async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync();
	}
}
