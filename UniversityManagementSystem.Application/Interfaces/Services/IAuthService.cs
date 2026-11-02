using UniversityManagementSystem.Application.DTOs.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto model);
        Task LogoutAsync();
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto model);
        Task<bool> ChangePasswordAsync(ChangePasswordDto model);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
    }
}