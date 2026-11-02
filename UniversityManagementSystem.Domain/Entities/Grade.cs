using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Grade : BaseEntity
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public GradeType Type { get; set; }
        public int? ExamId { get; set; }
        public int? AssignmentId { get; set; }
        public int? SubmissionId { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public string? LetterGrade { get; set; } = string.Empty;
        public string? Remarks { get; set; } = string.Empty;
        public int? Semester { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
        public virtual Exam Exam { get; set; } = null!;
        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Submission Submission { get; set; } = null!;
    }
}