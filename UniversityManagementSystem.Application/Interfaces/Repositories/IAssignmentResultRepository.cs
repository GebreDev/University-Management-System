using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IAssignmentResultRepository : IRepository<AssignmentResult>
    {
        Task<IEnumerable<AssignmentResult>> GetByAssignmentIdAsync(int assignmentId);
        Task<AssignmentResult?> GetByStudentAndAssignmentAsync(int studentId, int assignmentId);
        Task<IEnumerable<AssignmentResult>> GetByStudentIdAsync(int studentId);
    }
}
