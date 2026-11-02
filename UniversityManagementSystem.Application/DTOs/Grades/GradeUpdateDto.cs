using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Grades
{
    public class GradeUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal Score { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal MaxScore { get; set; }

        public string Remarks { get; set; }
    }
}