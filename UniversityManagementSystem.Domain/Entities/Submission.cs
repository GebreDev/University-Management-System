using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Submission : BaseEntity
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? FilePath { get; set; } = string.Empty;
        public int? Grade { get; set; }
        public string? Feedback { get; set; } = string.Empty;
        public bool IsGraded { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}