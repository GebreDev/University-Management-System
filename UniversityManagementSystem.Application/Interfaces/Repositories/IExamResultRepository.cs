using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IExamResultRepository : IRepository<ExamResult>
    {
        Task<IEnumerable<ExamResult>> GetByExamIdAsync(int examId);
        Task<ExamResult?> GetByStudentAndExamAsync(int studentId, int examId);
        Task<IEnumerable<ExamResult>> GetByStudentIdAsync(int studentId);
    }
}