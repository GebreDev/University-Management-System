using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class CourseMaterialRepository : GenericRepository<CourseMaterial>, ICourseMaterialRepository, IRepository<CourseMaterial>
{
	public CourseMaterialRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<CourseMaterial>?> GetByCourseIdAsync(int courseId)
	{
		return await (from m in _dbSet.AsNoTracking().Include((CourseMaterial m) => m.Course).Include((CourseMaterial m) => m.UploadedBy)
			where m.CourseId == courseId
			orderby m.CreatedAt descending
			select m).ToListAsync();
	}

	public async Task<IEnumerable<CourseMaterial>?> GetByUploaderIdAsync(string uploaderId)
	{
		return await (from m in _dbSet.AsNoTracking().Include((CourseMaterial m) => m.Course)
			where m.UploadedById == uploaderId
			orderby m.CreatedAt descending
			select m).ToListAsync();
	}

	public async Task<IEnumerable<CourseMaterial>?> GetByCourseIdsAsync(IEnumerable<int> courseIds)
	{
		List<int> ids = courseIds.ToList();
		if (ids.Count == 0)
		{
			return Enumerable.Empty<CourseMaterial>();
		}
		return await (from m in _dbSet.AsNoTracking().Include((CourseMaterial m) => m.Course).Include((CourseMaterial m) => m.UploadedBy)
			where ids.Contains(m.CourseId)
			orderby m.CreatedAt descending
			select m).ToListAsync();
	}

	public async Task<CourseMaterial?> GetByIdWithDetailsAsync(int id)
	{
		return await _dbSet.AsNoTracking().Include((CourseMaterial m) => m.Course).Include((CourseMaterial m) => m.UploadedBy)
			.FirstOrDefaultAsync((CourseMaterial m) => m.Id == id);
	}
}
