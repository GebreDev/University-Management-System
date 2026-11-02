using UniversityManagementSystem.Domain.Common;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// The mark a student scored on a specific course assessment component.
    /// The mark cannot exceed the component's Weight (which is its max mark).
    /// </summary>
    public class AssessmentMark : BaseEntity
    {
        public int CourseAssessmentId { get; set; }
        public int StudentId { get; set; }
        public decimal Score { get; set; }

        public virtual CourseAssessment CourseAssessment { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
