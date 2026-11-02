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
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new RoleCreateViewModel();
            ViewBag.Permissions = Permissions.All;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.RoleName))
                ModelState.AddModelError(nameof(model.RoleName), "Role name is required.");

            if (!ModelState.IsValid)
            {
                ViewBag.Permissions = Permissions.All;
                return View(model);
            }

            model.RoleName = model.RoleName.Trim();

            var existingRole = await _roleManager.FindByNameAsync(model.RoleName);
            if (existingRole != null)
            {
                ModelState.AddModelError(nameof(model.RoleName), "This role already exists.");
                ViewBag.Permissions = Permissions.All;
                return View(model);
            }

            var role = new IdentityRole(model.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                ViewBag.Permissions = Permissions.All;
                return View(model);
            }

            if (model.SelectedPermissions != null && model.SelectedPermissions.Count > 0)
            {
                var validPermissions = model.SelectedPermissions
                    .Where(permission => Permissions.All.Contains(permission))
                    .Distinct()
                    .ToList();

                foreach (var permission in validPermissions)
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        Permission = permission
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Role '{model.RoleName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}