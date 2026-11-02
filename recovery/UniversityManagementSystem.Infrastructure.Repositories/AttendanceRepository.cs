using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository, IRepository<Attendance>
{
	public AttendanceRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public override async Task<Attendance?> GetByIdAsync(int id)
	{
		return await _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course).FirstOrDefaultAsync((Attendance a) => a.Id == id);
	}

	public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId)
	{
		return await (from a in _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course)
			where a.StudentId == studentId
			select a).ToListAsync();
	}

	public async Task<IEnumerable<Attendance>> GetByCourseIdAsync(int courseId)
	{
		return await (from a in _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course)
			where a.CourseId == courseId
			select a).ToListAsync();
	}

	public async Task<IEnumerable<Attendance>> GetByCourseIdsAsync(IEnumerable<int> courseIds)
	{
		List<int> ids = courseIds.ToList();
		if (!ids.Any())
		{
			return new List<Attendance>();
		}
		return await (from a in _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course)
			where ids.Contains(a.CourseId)
			select a).ToListAsync();
	}

	public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date)
	{
		return await (from a in _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course)
			where a.Date.Date == ((DateTime)date).Date
			select a).ToListAsync();
	}

	public async Task<IEnumerable<Attendance>> GetByStudentAndMonthAsync(int studentId, int year, int month)
	{
		return await (from a in _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course)
			where a.StudentId == studentId && a.Date.Year == year && a.Date.Month == month
			select a).ToListAsync();
	}

	public async Task<Attendance?> GetByStudentAndDateAsync(int studentId, int courseId, DateTime date)
	{
		return await _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course).FirstOrDefaultAsync((Attendance a) => a.StudentId == studentId && a.CourseId == courseId && a.Date.Date == ((DateTime)date).Date);
	}

	public async Task<IEnumerable<Attendance>> GetAbsentStudentsAsync(int courseId)
	{
		return await (from a in _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course)
			where a.CourseId == courseId && (int)a.Status == 2
			select a).ToListAsync();
	}

	public async Task<IEnumerable<Attendance>> GetByInstructorIdAsync(int instructorId)
	{
		return await (from a in _dbSet.Include((Attendance a) => a.Student).Include((Attendance a) => a.Course)
			where a.InstructorId == (int?)instructorId
			select a).ToListAsync();
	}
}
