using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Enrollment
{
    public class EnrollmentUpdateDto
    {
        [Required]
        public int Id { get; set; }

        // Only status can be updated by admin? We'll allow status update.
        public string Status { get; set; } // "Approved", "Rejected", "Completed"
        public string RejectionReason { get; set; }
    }
}