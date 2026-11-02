using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetStudentWithDetailsAsync(int id);
        Task<IEnumerable<Student>> GetAllStudentsWithDetailsAsync();
        Task<Student?> GetByStudentIdAsync(string studentId);
        Task<Student?> GetByApplicationUserIdAsync(string userId);
        Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId);
        Task<IEnumerable<Student>> GetBySectionAsync(int sectionId);

        // ===== Finance support =====
        Task<Student?> GetStudentWithUserAsync(int id);

        /// <summary>Semester with its academic year (used by fee assessment).</summary>
        Task<Semester?> GetSemesterWithYearAsync(int semesterId);

        /// <summary>Single-row finance settings, created with defaults on first use.</summary>
        Task<FinanceSetting> GetFinanceSettingsAsync();
    }
}