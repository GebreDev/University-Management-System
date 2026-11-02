using UniversityManagementSystem.Application.DTOs.Account;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IInstructorRepository _instructorRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IStudentRepository studentRepository,
            IInstructorRepository instructorRepository,
            IDepartmentRepository departmentRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _studentRepository = studentRepository;
            _instructorRepository = instructorRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<bool> LoginAsync(LoginDto model)
        {
            // Try username first, then fall back to email for any legacy users.
            var user = await _userManager.FindByNameAsync(model.Username)
                ?? await _userManager.FindByEmailAsync(model.Username);

            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto model)
        {
            var errors = new List<string>();

            // Check if username or email already exists
            var existingByUsername = await _userManager.FindByNameAsync(model.Username);
            if (existingByUsername != null)
            {
                errors.Add("This username is already taken. Please choose a different username.");
                return (false, errors);
            }

            var existingByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingByEmail != null)
            {
                errors.Add("This email is already registered. Please use a different email.");
                return (false, errors);
            }

            // If registering as Student, validate StudentId uniqueness
            if (model.Role == "Student" && !string.IsNullOrEmpty(model.StudentId))
            {
                var existingStudent = await _studentRepository.GetByStudentIdAsync(model.StudentId);
                if (existingStudent != null)
                {
                    // Polite and clear error message
                    errors.Add($"The Student ID '{model.StudentId}' is already in use. Please choose a different ID.");
                    return (false, errors);
                }
            }

            // Create user — Username is the login credential.
            // For Students the username is the StudentId (UGR); for Instructors it is provided explicitly.
            var userName = model.Username;
            if (string.IsNullOrEmpty(userName))
            {
                userName = model.Role == "Student" && !string.IsNullOrEmpty(model.StudentId)
                    ? model.StudentId
                    : model.Email;
            }

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address ?? "",
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                errors.AddRange(result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            // Assign role
            if (!string.IsNullOrEmpty(model.Role))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    errors.AddRange(roleResult.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                // Create Student or Instructor entity
                if (model.Role == "Student")
                {
                    await CreateStudentAsync(user.Id, model);
                }
                else if (model.Role == "Instructor")
                {
                    await CreateInstructorAsync(user.Id, model);
                }
            }

            return (true, Enumerable.Empty<string>());
        }

        private async Task CreateStudentAsync(string userId, RegisterDto model)
        {
            var departmentId = model.DepartmentId ?? await GetDefaultDepartmentId();

            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.PhoneNumber ?? "",
                EnrollmentDate = model.EnrollmentDate ?? DateTime.UtcNow,
                StudentId = model.StudentId ?? GenerateStudentId(),
                DepartmentId = departmentId,
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = EntityStatus.Active
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();
        }

        private async Task CreateInstructorAsync(string userId, RegisterDto model)
        {
            var departmentId = model.DepartmentId ?? await GetDefaultDepartmentId();

            var instructor = new Instructor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.PhoneNumber ?? "",
                Qualification = model.Qualification ?? "",
                HireDate = model.HireDate ?? DateTime.UtcNow,
                DepartmentId = departmentId,
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = EntityStatus.Active
            };

            await _instructorRepository.AddAsync(instructor);
            await _instructorRepository.SaveChangesAsync();
        }

        private async Task<int> GetDefaultDepartmentId()
        {
            var departments = await _departmentRepository.GetAllDepartmentsWithDetailsAsync();
            var dept = departments.FirstOrDefault();
            if (dept == null)
            {
                throw new Exception("No departments found. Please seed the database first.");
            }
            return dept.Id;
        }

        private string GenerateStudentId()
        {
            return $"STU{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}";
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await _userManager.GetUserAsync(_signInManager.Context.User);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username)
                ?? await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // In a real app, send email with reset link
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username)
                ?? await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            return result.Succeeded;
        }
    }
}