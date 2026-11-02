using UniversityManagementSystem.Application.DTOs.Account;
using UniversityManagementSystem.Application.DTOs.UserManagement;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserManagementService _userService;
        private readonly IFileService _fileService;
        private readonly IAuditLogService _auditLogService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IDepartmentService _departmentService;

        public AccountController(
            IAuthService authService,
            IUserManagementService userService,
            IFileService fileService,
            IAuditLogService auditLogService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IDepartmentService departmentService)
        {
            _authService = authService;
            _userService = userService;
            _fileService = fileService;
            _auditLogService = auditLogService;
            _signInManager = signInManager;
            _userManager = userManager;
            _departmentService = departmentService;
        }

        // === Login ===
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToDashboard();
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.HideWelcome = true;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.HideWelcome = true;
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model);
                if (result)
                {
                    var user = await _userManager.FindByNameAsync(model.Username)
                        ?? await _userManager.FindByEmailAsync(model.Username);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", roles.FirstOrDefault() ?? "", "Login", "Account", null, "User logged in");
                    }
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                await _auditLogService.LogAsync(
                    null,
                    string.IsNullOrWhiteSpace(model.Username) ? "Unknown" : model.Username,
                    string.Empty,
                    "LoginFailed",
                    "Account",
                    null,
                    "Failed login attempt");
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            else
            {
                await _auditLogService.LogAsync(
                    null,
                    string.IsNullOrWhiteSpace(model.Username) ? "Unknown" : model.Username,
                    string.Empty,
                    "LoginFailed",
                    "Account",
                    null,
                    "Login validation failed");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", roles.FirstOrDefault() ?? "", "Logout", "Account", null, "User logged out");
            }
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        // === Register ===
        // Public registration is disabled. Students and instructors are provisioned
        // by the admin with credentials, as in a real university portal.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return NotFound();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDto model)
        {
            return NotFound();
        }

        // === Profile Management ===
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var profile = await _userService.GetUserProfileAsync(user.Id);
            return View(profile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileDto model)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateProfileAsync(model);
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }
            return View("Profile", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(ProfilePictureDto model)
        {
            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                TempData["Error"] = "No image selected.";
                return RedirectToAction(nameof(Profile));
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            try
            {
                var filePath = await _fileService.SaveProfilePictureAsync(model.ImageFile, user.Id);
                await _userService.UpdateProfilePictureAsync(user.Id, filePath ?? string.Empty);
                TempData["Success"] = "Profile picture updated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to upload that image: {ex.Message}";
            }
            if (User.IsInRole("Instructor"))
                return RedirectToAction("Profile", "Instructor");
            if (User.IsInRole("Student"))
                return RedirectToAction("Profile", "Student");
            return RedirectToAction(nameof(Profile));
        }

        // === Change Password ===
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ChangePasswordAsync(model);
                if (result)
                {
                    TempData["Success"] = "Password changed successfully.";
                    return RedirectToAction(nameof(Profile));
                }
                ModelState.AddModelError(string.Empty, "Failed to change password.");
            }
            return View(model);
        }

        // === Forgot / Reset Password ===
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username)
                    ?? await _userManager.FindByEmailAsync(model.Username);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { token, username = model.Username }, Request.Scheme);

                    // In production, send email via SMTP/MailKit/SendGrid
                    // For demo/development, show the link in TempData
                    TempData["ResetLink"] = resetLink;
                    TempData["Success"] = "A password reset link has been generated. Check the demo link below.";
                }
                else
                {
                    TempData["Success"] = "If an account with that username exists, a reset link has been sent.";
                }
                return RedirectToAction(nameof(ForgotPassword));
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string username)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
                return RedirectToAction(nameof(Login));
            return View(new ResetPasswordDto { Token = token, Username = username });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ResetPasswordAsync(model);
                if (result)
                {
                    TempData["Success"] = "Password reset successfully. Please login.";
                    return RedirectToAction(nameof(Login));
                }
                ModelState.AddModelError(string.Empty, "Invalid reset attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToDashboard()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Dashboard", "Admin");
            if (User.IsInRole("Instructor"))
                return RedirectToAction("Dashboard", "Instructor");
            if (User.IsInRole("Student"))
                return RedirectToAction("Dashboard", "Student");
            return RedirectToAction("Index", "Home");
        }
    }
}
