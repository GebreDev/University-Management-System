using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Application.DTOs.Exams
{
    public class StudentAnswerGradingDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionMarks { get; set; }
        public ExamType QuestionType { get; set; }
        public string AnswerText { get; set; }
        public int? Score { get; set; }
        public bool IsCorrect { get; set; }
    }
}
