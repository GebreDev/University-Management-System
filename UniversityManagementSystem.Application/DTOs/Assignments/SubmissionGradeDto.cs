using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Assignments
{
    public class SubmissionGradeDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        [Range(0, 1000)]
        public int Grade { get; set; }

        public string Feedback { get; set; }
    }
}
