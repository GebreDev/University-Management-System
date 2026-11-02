using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Registration
{
    public class RegistrationReviewDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string NewStatus { get; set; } = string.Empty;

        public string? RejectionReason { get; set; }

        public string? ReviewedBy { get; set; }
    }
}
