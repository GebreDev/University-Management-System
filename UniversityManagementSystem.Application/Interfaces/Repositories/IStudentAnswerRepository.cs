using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IStudentAnswerRepository : IRepository<StudentAnswer>
    {
        Task<IEnumerable<StudentAnswer>> GetByStudentAndExamAsync(int studentId, int examId);
        Task<IEnumerable<StudentAnswer>> GetByQuestionIdAsync(int questionId);
    }
}