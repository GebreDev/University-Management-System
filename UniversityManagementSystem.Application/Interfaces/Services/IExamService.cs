using UniversityManagementSystem.Application.DTOs.Exams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IExamService
    {
        Task<ExamDto> CreateExamAsync(ExamDto dto);
        Task<ExamDto> UpdateExamAsync(ExamDto dto);
        Task DeleteExamAsync(int examId);
        Task<ExamDto> GetExamByIdAsync(int examId);
        Task<ExamDto> GetExamWithQuestionsAsync(int examId);
        Task<IEnumerable<ExamDto>> GetExamsByCourseIdAsync(int courseId);
        Task<IEnumerable<ExamDto>> GetExamsByInstructorIdAsync(int instructorId);
        Task<IEnumerable<ExamDto>> GetExamsForStudentAsync(int studentId);
        Task PublishExamAsync(int examId);

        Task<QuestionDto> AddQuestionAsync(QuestionDto dto);
        Task UpdateQuestionAsync(QuestionDto dto);
        Task DeleteQuestionAsync(int questionId);
        Task<QuestionDto?> GetQuestionWithOptionsAsync(int questionId);

        Task<ExamResultDto> GradeExamAsync(int examId, int studentId, Dictionary<int, string> answers);
        Task<ExamResultDto> GetResultByIdAsync(int examResultId);
        Task<ExamResultDto?> GetResultByStudentAndExamAsync(int studentId, int examId);
        Task<IEnumerable<ExamResultDto>> GetResultsByExamIdAsync(int examId);
        Task SubmitExamAsync(int examId, int studentId, Dictionary<int, string> answers);
        Task<IEnumerable<ExamResultDto>> GetSubmissionsForExamAsync(int examId);

        // Manual essay grading
        Task<IEnumerable<StudentAnswerGradingDto>> GetStudentAnswersForGradingAsync(int examId, int studentId);
        Task<ExamResultDto> ManualGradeEssayAsync(int examId, int studentId, Dictionary<int, int> scores);
        Task<ExamResultDto> UpdateManualExamResultAsync(int examResultId, int totalScore, string remarks);
    }
}