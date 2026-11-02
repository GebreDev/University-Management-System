using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface ISubmissionRepository : IRepository<Submission>
    {
        Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<Submission>> GetByAssignmentIdsAsync(IEnumerable<int> assignmentIds);
        Task<IEnumerable<Submission>> GetByStudentIdAsync(int studentId);
        Task<Submission?> GetByStudentAndAssignmentAsync(int studentId, int assignmentId);
        Task<IEnumerable<Submission>> GetGradedSubmissionsAsync(int assignmentId);
        Task<IEnumerable<Submission>> GetUngradedSubmissionsAsync(int assignmentId);
    }
}