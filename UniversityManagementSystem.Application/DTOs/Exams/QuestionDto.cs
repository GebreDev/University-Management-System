using UniversityManagementSystem.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Exams
{
    public class QuestionDto
    {
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public ExamType Type { get; set; }

        [Required]
        [Range(1, 100)]
        public int Marks { get; set; }

        public string CorrectAnswer { get; set; } // For MCQ, this is the correct option text

        // New MCQ fields
        public string? Explanation { get; set; }
        public string? DifficultyLevel { get; set; } // e.g. Easy, Medium, Hard

        public List<QuestionOptionDto> Options { get; set; } // For MCQ
    }

    public class QuestionOptionDto
    {
        public int Id { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}