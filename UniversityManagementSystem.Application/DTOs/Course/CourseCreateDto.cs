using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Course
{
    public class CourseCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [Range(1, 10)]
        public int Credits { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int InstructorId { get; set; }
    }
}