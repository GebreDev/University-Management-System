using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface ICourseAssessmentRepository : IRepository<CourseAssessment>
    {
        Task<IEnumerable<CourseAssessment>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<CourseAssessment>> GetByCourseIdWithMarksAsync(int courseId);
        Task<IEnumerable<CourseAssessment>> GetByCourseIdsWithMarksAsync(IEnumerable<int> courseIds);
        Task DeleteByCourseIdAsync(int courseId);
    }
}
