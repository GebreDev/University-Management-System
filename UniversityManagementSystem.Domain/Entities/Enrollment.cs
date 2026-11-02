using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        public DateTime EnrollmentDate { get; set; }
        public new EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
        public DateTime? ApprovalDate { get; set; }
        public string? RejectionReason { get; set; }  // <-- Made nullable

        // Foreign keys
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
    }
}