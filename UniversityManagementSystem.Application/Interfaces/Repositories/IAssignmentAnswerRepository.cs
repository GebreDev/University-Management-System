using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IAssignmentAnswerRepository : IRepository<AssignmentAnswer>
    {
        Task<IEnumerable<AssignmentAnswer>> GetByStudentAndAssignmentAsync(int studentId, int assignmentId);
        Task<IEnumerable<AssignmentAnswer>> GetByAssignmentQuestionIdAsync(int questionId);
    }
}
