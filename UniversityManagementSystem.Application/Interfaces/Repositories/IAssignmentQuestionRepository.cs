using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IAssignmentQuestionRepository : IRepository<AssignmentQuestion>
    {
        Task<IEnumerable<AssignmentQuestion>> GetByAssignmentIdAsync(int assignmentId);
        Task<AssignmentQuestion?> GetByIdWithOptionsAsync(int questionId);
    }
}
