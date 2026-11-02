using UniversityManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Attendance>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<Attendance>> GetByCourseIdsAsync(IEnumerable<int> courseIds);
        Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date);
        Task<IEnumerable<Attendance>> GetByStudentAndMonthAsync(int studentId, int year, int month);
        Task<Attendance?> GetByStudentAndDateAsync(int studentId, int courseId, DateTime date);
        Task<IEnumerable<Attendance>> GetAbsentStudentsAsync(int courseId);
        Task<IEnumerable<Attendance>> GetByInstructorIdAsync(int instructorId);
    }
}