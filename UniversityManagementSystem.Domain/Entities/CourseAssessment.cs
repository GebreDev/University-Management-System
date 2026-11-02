using UniversityManagementSystem.Domain.Common;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// An assessment component (column) defined by an instructor for a course,
    /// e.g. "Test (25%)", "Assignment and Quiz (20%)", "Lab (15%)", "Final Exam (40%)".
    /// The Weight is the percentage contribution to the final grade (out of 100),
    /// and also the maximum mark a student can score for this component.
    /// </summary>
    public class CourseAssessment : BaseEntity
    {
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int? Semester { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<AssessmentMark> Marks { get; set; } = new List<AssessmentMark>();
    }
}
