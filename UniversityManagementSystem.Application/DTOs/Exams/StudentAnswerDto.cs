using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Exams
{
    public class StudentAnswerDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; }

        public int? Score { get; set; }
        public bool IsCorrect { get; set; }
    }
}