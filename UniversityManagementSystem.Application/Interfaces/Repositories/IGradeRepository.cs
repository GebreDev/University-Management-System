using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IGradeRepository : IRepository<Grade>
    {
        Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Grade>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<Grade>> GetByStudentAndCourseAsync(int studentId, int courseId);
        Task<IEnumerable<Grade>> GetByExamIdAsync(int examId);
        Task<IEnumerable<Grade>> GetByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<Grade>> GetByCourseIdsAsync(IEnumerable<int> courseIds);
    }
}