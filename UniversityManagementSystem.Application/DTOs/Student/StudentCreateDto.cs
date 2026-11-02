using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Student
{
    public class StudentCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }

        [Required]
        public string StudentId { get; set; } // Unique

        [Required]
        public int DepartmentId { get; set; }

        // For user creation
        [Required]
        public string Password { get; set; }

        // Login username — defaults to StudentId (UGR) if not supplied
        public string? Username { get; set; }
    }
}