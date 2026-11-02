using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;
using UniversityManagementSystem.Web.Authorization;
using UniversityManagementSystem.Web.Models.ViewModels;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize]
    [Permission(Permissions.UserManagement)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var users = new List<UserListViewModel>();
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(search)) ||
                    (u.LastName != null && u.LastName.Contains(search)) ||
                    (u.Email != null && u.Email.Contains(search)) ||
                    (u.UserName != null && u.UserName.Contains(search))
                );
            }

            foreach (var user in await query.OrderBy(u => u.LastName).ToListAsync())
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "No Role";

                int? profileId = null;
                if (string.Equals(role, "Student", StringComparison.OrdinalIgnoreCase))
                {
                    profileId = await _context.Students
                        .Where(s => s.ApplicationUserId == user.Id)
                        .Select(s => (int?)s.Id)
                        .FirstOrDefaultAsync();
                }
                else if (string.Equals(role, "Instructor", StringComparison.OrdinalIgnoreCase))
                {
                    profileId = await _context.Instructors
                        .Where(t => t.ApplicationUserId == user.Id)
                        .Select(t => (int?)t.Id)
                        .FirstOrDefaultAsync();
                }

                users.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Role = role,
                    ProfileId = profileId
                });
            }

            ViewBag.Search = search;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (string.Equals(model.Role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Success"] = "Students are registered from the Student registration form.";
                return RedirectToAction("CreateStudent", "Admin");
            }

            if (string.Equals(model.Role, "Instructor", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Success"] = "Instructors are registered from the Instructor registration form.";
                return RedirectToAction("CreateInstructor", "Admin");
            }

            if (!ModelState.IsValid)
            {
                await LoadRolesAsync();
                return View(model);
            }

            var existingUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUsername != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "Username already exists.");
                await LoadRolesAsync();
                return View(model);
            }

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");
                await LoadRolesAsync();
                return View(model);
            }

            // Split full name into first and last name
            var nameParts = model.FullName.Trim().Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                await LoadRolesAsync();
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Role))
            {
                if (string.Equals(model.Role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                    !User.IsInRole("Admin"))
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(nameof(model.Role), "Only an administrator can assign the Admin role.");
                    await LoadRolesAsync();
                    return View(model);
                }

                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(nameof(model.Role), "Selected role does not exist.");
                    await LoadRolesAsync();
                    return View(model);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    await _userManager.DeleteAsync(user);
                    await LoadRolesAsync();
                    return View(model);
                }
            }

            TempData["Success"] = $"User '{user.UserName}' was created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault() ?? "";

            if (string.Equals(currentRole, "Student", StringComparison.OrdinalIgnoreCase))
            {
                var studentId = await _context.Students
                    .Where(s => s.ApplicationUserId == user.Id)
                    .Select(s => (int?)s.Id)
                    .FirstOrDefaultAsync();
                if (studentId != null)
                    return RedirectToAction("EditStudent", "Admin", new { id = studentId });
            }

            if (string.Equals(currentRole, "Instructor", StringComparison.OrdinalIgnoreCase))
            {
                var instructorId = await _context.Instructors
                    .Where(t => t.ApplicationUserId == user.Id)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefaultAsync();
                if (instructorId != null)
                    return RedirectToAction("EditInstructor", "Admin", new { id = instructorId });
            }

            await LoadRolesAsync();
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Role = currentRole
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Protect the default admin username
            if (string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
                model.UserName = "admin";

            var usernameOwner = await _userManager.FindByNameAsync(model.UserName);
            if (usernameOwner != null && usernameOwner.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.UserName), "Username already exists.");
                await LoadRolesAsync();
                return View(model);
            }

            var emailOwner = await _userManager.FindByEmailAsync(model.Email);
            if (emailOwner != null && emailOwner.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");
                await LoadRolesAsync();
                return View(model);
            }

            // Split full name into first and last name
            var nameParts = model.FullName.Trim().Split(' ', 2);
            user.FirstName = nameParts[0];
            user.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            user.UserName = model.UserName;
            user.Email = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);
                await LoadRolesAsync();
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Role))
            {
                if (string.Equals(model.Role, "Student", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(model.Role, "Instructor", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(model.Role),
                        $"{model.Role} accounts must be created from the {model.Role} registration form.");
                    await LoadRolesAsync();
                    return View(model);
                }

                if (string.Equals(model.Role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                    !User.IsInRole("Admin"))
                {
                    ModelState.AddModelError(nameof(model.Role), "Only an administrator can assign the Admin role.");
                    await LoadRolesAsync();
                    return View(model);
                }

                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    ModelState.AddModelError(nameof(model.Role), "Selected role does not exist.");
                    await LoadRolesAsync();
                    return View(model);
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Count > 0)
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                            ModelState.AddModelError("", error.Description);
                        await LoadRolesAsync();
                        return View(model);
                    }
                }

                var addResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    await LoadRolesAsync();
                    return View(model);
                }
            }

            TempData["Success"] = $"User '{user.UserName}' was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            if (string.Equals(role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                var studentId = await _context.Students
                    .Where(s => s.ApplicationUserId == user.Id)
                    .Select(s => (int?)s.Id)
                    .FirstOrDefaultAsync();
                if (studentId != null)
                    return RedirectToAction("DeleteStudent", "Admin", new { id = studentId });
            }

            if (string.Equals(role, "Instructor", StringComparison.OrdinalIgnoreCase))
            {
                var instructorId = await _context.Instructors
                    .Where(t => t.ApplicationUserId == user.Id)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefaultAsync();
                if (instructorId != null)
                    return RedirectToAction("DeleteInstructor", "Admin", new { id = instructorId });
            }

            var model = new UserListViewModel
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                Role = role
            };
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "The default administrator cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            if (string.Equals(role, "Student", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Instructor", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = $"{role} accounts must be deleted from the {role} management page.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                TempData["Error"] = errors;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new ResetPasswordViewModel
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email ?? ""
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            TempData["Success"] = $"Password for '{user.UserName}' was reset successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRolesAsync()
        {
            ViewBag.Roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }
    }
}