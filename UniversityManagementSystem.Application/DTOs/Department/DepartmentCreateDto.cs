using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Department
{
    public class DepartmentCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }
    }
}