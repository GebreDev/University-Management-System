using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}