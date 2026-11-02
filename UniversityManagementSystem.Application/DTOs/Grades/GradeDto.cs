using UniversityManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Grades
{
    public class GradeDto
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public GradeType Type { get; set; }

        public int? ExamId { get; set; }
        public int? AssignmentId { get; set; }
        public int? SubmissionId { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal Score { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal MaxScore { get; set; }

        public string LetterGrade { get; set; }
        public string Remarks { get; set; }
        public int? Semester { get; set; }
    }
}