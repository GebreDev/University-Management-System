using UniversityManagementSystem.Application.DTOs.CourseMaterial;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniversityManagementSystem.Domain.Entities;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize]
    public class CourseMaterialController : Controller
    {
        private readonly ICourseMaterialService _materialService;
        private readonly IInstructorService _instructorService;
        private readonly IStudentService _studentService;
        private readonly IUserManagementService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CourseMaterialController(
            ICourseMaterialService materialService,
            IInstructorService instructorService,
            IStudentService studentService,
            IUserManagementService userService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _materialService = materialService;
            _instructorService = instructorService;
            _studentService = studentService;
            _userService = userService;
            _userManager = userManager;
            _context = context;
        }

        private async Task<(string UserId, bool IsAdmin)> GetActorAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new System.UnauthorizedAccessException("User session expired. Please sign in again.");

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return (user.Id, isAdmin);
        }

        // ============================================================
        // INSTRUCTOR: manage materials
        // ============================================================

        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Index()
        {
            var (userId, isAdmin) = await GetActorAsync();

            if (isAdmin)
            {
                // Admins see all materials grouped by course.
                var all = await _context.CourseMaterials
                    .Include(m => m.Course)
                    .Include(m => m.UploadedBy)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return View(all.Select(m => new CourseMaterialListDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    FileName = m.FileName,
                    FileType = m.FileType,
                    FileSize = m.FileSize,
                    DownloadCount = m.DownloadCount,
                    CreatedAt = m.CreatedAt,
                    CourseId = m.CourseId,
                    CourseTitle = m.Course?.Title ?? string.Empty,
                    CourseCode = m.Course?.Code ?? string.Empty,
                    UploadedByName = m.UploadedBy != null ? $"{m.UploadedBy.FirstName} {m.UploadedBy.LastName}".Trim() : string.Empty
                }));
            }

            var materials = await _materialService.GetForInstructorAsync(userId);
            return View(materials);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            await PopulateCourseDropdownAsync();
            return View(new CourseMaterialUploadDto());
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(CourseMaterialUploadDto dto)
        {
            var (userId, isAdmin) = await GetActorAsync();

            if (!ModelState.IsValid)
            {
                await PopulateCourseDropdownAsync();
                return View(dto);
            }

            try
            {
                // Non-admin instructors may only upload to their own courses.
                if (!isAdmin)
                {
                    var instructor = await _instructorService.GetInstructorByUserIdAsync(userId);
                    if (instructor == null)
                    {
                        ModelState.AddModelError(string.Empty, "Instructor profile not found.");
                        await PopulateCourseDropdownAsync();
                        return View(dto);
                    }

                    var ownsCourse = await _context.Courses
                        .AnyAsync(c => c.Id == dto.CourseId && c.InstructorId == instructor.Id);
                    if (!ownsCourse)
                    {
                        ModelState.AddModelError(string.Empty, "You can only upload materials for your own courses.");
                        await PopulateCourseDropdownAsync();
                        return View(dto);
                    }
                }

                var result = await _materialService.UploadAsync(dto, userId);
                TempData["Success"] = $"Course material '{result.Title}' uploaded successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateCourseDropdownAsync();
                return View(dto);
            }
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (userId, isAdmin) = await GetActorAsync();
            if (!await _materialService.CanManageAsync(id, userId, isAdmin))
                return Forbid();

            var material = await _materialService.GetByIdAsync(id, userId, isAdmin);
            return View(new CourseMaterialUpdateDto
            {
                Id = material.Id,
                Title = material.Title,
                Description = material.Description
            });
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Edit(int id, CourseMaterialUpdateDto dto)
        {
            var (userId, isAdmin) = await GetActorAsync();
            if (id != dto.Id)
                return BadRequest();
            if (!await _materialService.CanManageAsync(id, userId, isAdmin))
                return Forbid();

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _materialService.UpdateAsync(dto, userId);
                TempData["Success"] = "Course material updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (userId, isAdmin) = await GetActorAsync();
            if (!await _materialService.CanManageAsync(id, userId, isAdmin))
                return Forbid();

            try
            {
                await _materialService.DeleteAsync(id, userId, isAdmin);
                TempData["Success"] = "Course material deleted successfully.";
            }
            catch (CustomException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var (userId, isAdmin) = await GetActorAsync();
            if (!await _materialService.CanManageAsync(id, userId, isAdmin))
                return Forbid();

            var material = await _materialService.GetByIdAsync(id, userId, isAdmin);
            return View(material);
        }

        // ============================================================
        // SHARED: download (instructor, admin, or enrolled student)
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var (userId, isAdmin) = await GetActorAsync();

            int? studentId = null;
            if (!isAdmin)
            {
                var student = await _studentService.GetStudentByUserIdAsync(userId);
                studentId = student?.Id;
            }

            var material = await _materialService.GetByIdAsync(id, userId, isAdmin, studentId);

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot",
                material.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "The file no longer exists on the server.";
                return RedirectToAction(nameof(Index));
            }

            await _materialService.IncrementDownloadCountAsync(id);

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/octet-stream", material.FileName);
        }

        // ============================================================
        // STUDENT: my course materials
        // ============================================================

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyMaterials()
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return RedirectToAction("Index", "Home");

            var materials = await _materialService.GetForStudentAsync(student.Id);
            return View(materials);
        }

        // ============================================================
        // Helpers
        // ============================================================

        private async Task PopulateCourseDropdownAsync()
        {
            var (userId, isAdmin) = await GetActorAsync();

            System.Collections.Generic.List<Course> courses;
            if (isAdmin)
            {
                courses = await _context.Courses
                    .OrderBy(c => c.Title)
                    .ToListAsync();
            }
            else
            {
                var instructor = await _instructorService.GetInstructorByUserIdAsync(userId);
                courses = instructor == null
                    ? new System.Collections.Generic.List<Course>()
                    : await _context.Courses
                        .Where(c => c.InstructorId == instructor.Id)
                        .OrderBy(c => c.Title)
                        .ToListAsync();
            }

            ViewBag.Courses = courses.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Code} — {c.Title}"                }).ToList();
        }
    }
}
