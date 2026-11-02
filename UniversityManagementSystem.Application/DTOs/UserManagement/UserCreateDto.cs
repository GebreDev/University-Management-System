using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.UserManagement
{
    public class UserCreateDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }

        // Login username — for students this should be the UGR/StudentId
        public string? Username { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}