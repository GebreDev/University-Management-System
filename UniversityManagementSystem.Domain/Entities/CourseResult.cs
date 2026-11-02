using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// The final, published result of a student for a course.
    /// Created when an instructor "sends" the assessment marks to students.
    /// Only published results (IsPublished == true) are visible to students.
    /// </summary>
    public class CourseResult : BaseEntity
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int? Semester { get; set; }
        public decimal TotalMark { get; set; }          // out of 100
        public string LetterGrade { get; set; } = string.Empty;
        public decimal GradePoint { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
