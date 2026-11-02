using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        Task<IEnumerable<Assignment>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<Assignment>> GetByCourseIdsAsync(IEnumerable<int> courseIds);
        Task<IEnumerable<Assignment>> GetPublishedAsync();
        Task<IEnumerable<Assignment>> GetByInstructorIdAsync(int instructorId);
    }
}