using UniversityManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Assignment : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public DateTime Deadline { get; set; }
        public int TotalMarks { get; set; }
        public string? AttachmentPath { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public bool AllowLateSubmission { get; set; }
        public int? LateSubmissionPenalty { get; set; } // Percentage penalty (e.g., 10 = 10% off)

        public virtual Course Course { get; set; } = null!;
        public virtual Instructor Instructor { get; set; } = null!;
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public virtual ICollection<AssignmentQuestion> AssignmentQuestions { get; set; } = new List<AssignmentQuestion>();
    }
}