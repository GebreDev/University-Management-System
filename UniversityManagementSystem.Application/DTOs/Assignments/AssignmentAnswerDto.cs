using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Application.DTOs.Assignments
{
    public class AssignmentAnswerDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignmentQuestionId { get; set; }
        public string AnswerText { get; set; }
        public int? Score { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class AssignmentAnswerGradingDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignmentQuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionMarks { get; set; }
        public ExamType QuestionType { get; set; }
        public string AnswerText { get; set; }
        public int? Score { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class AssignmentResultDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public string StudentName { get; set; }
        public string AssignmentTitle { get; set; }
        public int TotalScore { get; set; }
        public string Grade { get; set; }
        public bool IsPass { get; set; }
        public string Remarks { get; set; }
    }
}
