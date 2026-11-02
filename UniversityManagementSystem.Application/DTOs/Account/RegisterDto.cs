using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; }  // "Student" or "Instructor"

        // Student-specific fields
        public string? StudentId { get; set; }
        public DateTime? EnrollmentDate { get; set; }

        // Instructor-specific fields
        public string? Qualification { get; set; }
        public DateTime? HireDate { get; set; }

        // Common (for both)
        public int? DepartmentId { get; set; }
    }
}