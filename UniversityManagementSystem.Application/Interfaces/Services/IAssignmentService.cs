using UniversityManagementSystem.Application.DTOs.Assignments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IAssignmentService
    {
        // Assignments
        Task<AssignmentDto> CreateAssignmentAsync(AssignmentCreateDto dto, int instructorId);
        Task UpdateAssignmentAsync(AssignmentUpdateDto dto);
        Task DeleteAssignmentAsync(int id);
        Task PublishAssignmentAsync(int id);
        Task<IEnumerable<AssignmentDto>> GetAssignmentsByCourseIdAsync(int courseId);
        Task<IEnumerable<AssignmentDto>> GetAssignmentsForStudentAsync(int studentId);
        Task<AssignmentDto> GetAssignmentByIdAsync(int id);
        Task<IEnumerable<AssignmentDto>> GetAssignmentsByInstructorIdAsync(int instructorId);

        // Submissions
        Task SubmitAssignmentAsync(SubmissionCreateDto dto, int studentId);
        Task GradeSubmissionAsync(SubmissionGradeDto dto);
        Task<IEnumerable<SubmissionDto>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
        Task<SubmissionDto> GetSubmissionByStudentAndAssignmentAsync(int studentId, int assignmentId);
        Task<SubmissionDto> GetSubmissionByIdAsync(int submissionId);

        // Assignment Questions
        Task<AssignmentQuestionDto> AddAssignmentQuestionAsync(AssignmentQuestionDto dto);
        Task UpdateAssignmentQuestionAsync(AssignmentQuestionDto dto);
        Task DeleteAssignmentQuestionAsync(int questionId);
        Task<AssignmentQuestionDto?> GetAssignmentQuestionWithOptionsAsync(int questionId);

        // Assignment Answering & Grading
        Task<AssignmentResultDto> SubmitAssignmentAnswersAsync(int assignmentId, int studentId, Dictionary<int, string> answers);
        Task<AssignmentResultDto> GetAssignmentResultByIdAsync(int resultId);
        Task<AssignmentResultDto?> GetAssignmentResultByStudentAndAssignmentAsync(int studentId, int assignmentId);
        Task<IEnumerable<AssignmentResultDto>> GetAssignmentResultsByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<AssignmentAnswerGradingDto>> GetStudentAssignmentAnswersForGradingAsync(int assignmentId, int studentId);
        Task<AssignmentResultDto> ManualGradeAssignmentEssayAsync(int assignmentId, int studentId, Dictionary<int, int> scores);
        Task<AssignmentResultDto> UpdateAssignmentResultAsync(int resultId, int totalScore, string remarks);
    }
}