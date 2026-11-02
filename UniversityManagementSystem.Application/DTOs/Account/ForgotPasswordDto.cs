using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        public string Username { get; set; }
    }
}