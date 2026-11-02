using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<IEnumerable<Exam>> GetByCourseIdAsync(int courseId);
        Task<Exam?> GetWithQuestionsAsync(int examId);
        Task<IEnumerable<ExamResult>> GetResultsByExamIdAsync(int examId);
        Task<IEnumerable<ExamResult>> GetResultsByExamIdsAsync(IEnumerable<int> examIds);
        Task<ExamResult?> GetResultByStudentAndExamAsync(int studentId, int examId);
        Task<IEnumerable<Exam>> GetPublishedExamsAsync();
        Task<IEnumerable<Exam>> GetByInstructorIdAsync(int instructorId);
        Task<IEnumerable<Exam>> GetByDepartmentIdAsync(int departmentId);
    }
}