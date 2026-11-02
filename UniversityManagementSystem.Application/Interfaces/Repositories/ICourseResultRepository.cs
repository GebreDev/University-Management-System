using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface ICourseResultRepository : IRepository<CourseResult>
    {
        Task<IEnumerable<CourseResult>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<CourseResult>> GetPublishedByStudentIdAsync(int studentId);
        Task<IEnumerable<CourseResult>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<CourseResult>> GetByCourseIdsAsync(IEnumerable<int> courseIds);
        Task<CourseResult?> GetByStudentAndCourseAsync(int studentId, int courseId);
    }
}
