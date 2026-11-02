using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Instructor
{
    public class InstructorCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Qualification { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        // For user creation
        [Required]
        public string Password { get; set; }

        // Login username — if omitted, email is used as fallback
        public string? Username { get; set; }
    }
}