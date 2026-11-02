using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetCourseWithDetailsAsync(int id);
        Task<IEnumerable<Course>> GetAllCoursesWithDetailsAsync();
        Task<IEnumerable<Course>> GetCoursesByInstructorIdAsync(int instructorId);
        Task<IEnumerable<Course>> GetCoursesByDepartmentIdAsync(int departmentId);
        Task<Course?> GetByCodeAsync(string code);
        Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId);
    }
}