using UniversityManagementSystem.Application.DTOs.Registration;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityManagementSystem.Web.Views.Registration;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly ApplicationDbContext _context;

        public RegistrationController(
            IRegistrationService registrationService,
            ApplicationDbContext context)
        {
            _registrationService = registrationService;
            _context = context;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s!.Department)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);

            if (user?.Student == null)
                return RedirectToAction("Index", "Home");

            // Prefer semesters of the current academic year; if no year is
            // flagged IsCurrent (common right after setup), fall back to the
            // latest year by start date so registration is never dead-ended.
            var semesters = await _context.Semesters
                .Include(s => s.AcademicYear)
                .Where(s => s.AcademicYear!.IsCurrent)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            if (!semesters.Any())
            {
                var latestYearId = _context.AcademicYears
                    .OrderByDescending(y => y.StartDate)
                    .Select(y => (int?)y.Id)
                    .FirstOrDefault();

                if (latestYearId.HasValue)
                {
                    semesters = await _context.Semesters
                        .Include(s => s.AcademicYear)
                        .Where(s => s.AcademicYearId == latestYearId.Value)
                        .OrderByDescending(s => s.StartDate)
                        .ToListAsync();
                }
            }

            if (!semesters.Any())
            {
                semesters = await _context.Semesters
                    .Include(s => s.AcademicYear)
                    .OrderByDescending(s => s.StartDate)
                    .Take(4)
                    .ToListAsync();
            }

            ViewBag.Semesters = semesters;
            ViewBag.SelectedSemesterId = semesters.FirstOrDefault()?.Id;

            return View();
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChooseSemester(int semesterId)
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);

            if (user?.Student == null)
                return RedirectToAction("Index", "Home");

            var offerings = await _registrationService.GetOpenOfferingsBySemesterAsync(semesterId, user.Student.Id);
            var registrations = await _registrationService.GetStudentRegistrationsAsync(user.Student.Id);

            ViewBag.SemesterId = semesterId;
            ViewBag.SemesterName = await _context.Semesters.Where(s => s.Id == semesterId).Select(s => s.Name).FirstOrDefaultAsync();

            return View("Register", new RegisterViewModel
            {
                SemesterId = semesterId,
                Offerings = offerings.ToList(),
                ExistingRegistrations = registrations
            });
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> Register(int semesterId)
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);

            if (user?.Student == null)
                return RedirectToAction("Index", "Home");

            var offerings = await _registrationService.GetOpenOfferingsBySemesterAsync(semesterId, user.Student.Id);
            var registrations = await _registrationService.GetStudentRegistrationsAsync(user.Student.Id);

            ViewBag.SemesterId = semesterId;
            ViewBag.SemesterName = await _context.Semesters.Where(s => s.Id == semesterId).Select(s => s.Name).FirstOrDefaultAsync();

            return View(new RegisterViewModel
            {
                SemesterId = semesterId,
                Offerings = offerings.ToList(),
                ExistingRegistrations = registrations
            });
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRegistration(int semesterId, List<int> selectedOfferings)
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);

            if (user?.Student == null)
                return RedirectToAction("Index", "Home");

            try
            {
                var result = await _registrationService.SubmitRegistrationAsync(user.Student.Id, semesterId, selectedOfferings);
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(MyRegistrations));
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Register), new { semesterId });
            }
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyRegistrations()
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);

            if (user?.Student == null)
                return RedirectToAction("Index", "Home");

            var registrations = await _registrationService.GetStudentRegistrationsAsync(user.Student.Id);
            return View(registrations);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RegistrationDetails(int id)
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);

            if (user?.Student == null)
                return RedirectToAction("Index", "Home");

            var registration = await _registrationService.GetRegistrationAsync(id, user.Student.Id);
            if (registration == null)
                return NotFound();

            return View(registration);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);

            if (user?.Student == null)
                return RedirectToAction("Index", "Home");

            try
            {
                await _registrationService.WithdrawRegistrationAsync(id, user.Student.Id);
                TempData["Success"] = "Registration withdrawn.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(MyRegistrations));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var semesters = await _context.Semesters
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            ViewBag.Semesters = semesters;
            ViewBag.SelectedSemesterId = semesters.FirstOrDefault()?.Id;

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminSemesterRegistrations(int semesterId)
        {
            var registrations = await _registrationService.GetSemesterRegistrationsAsync(semesterId);
            var pending = await _registrationService.GetPendingRegistrationsAsync(semesterId);
            var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == semesterId);

            ViewBag.SemesterId = semesterId;
            ViewBag.SemesterName = semester?.Name ?? string.Empty;

            return View(new AdminSemesterRegistrationsViewModel
            {
                SemesterId = semesterId,
                Registrations = registrations.ToList(),
                PendingRegistrations = pending.ToList()
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminReview(int id)
        {
            var registration = await _registrationService.GetRegistrationForReviewAsync(id);
            if (registration == null)
                return NotFound();

            return View(registration);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminReview(RegistrationReviewDto dto)
        {
            var registration = await _registrationService.GetRegistrationForReviewAsync(dto.Id);
            if (registration == null)
                return NotFound();

            try
            {
                await _registrationService.ReviewRegistrationAsync(dto.Id, dto);
                TempData["Success"] = "Registration reviewed.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(AdminSemesterRegistrations), new { semesterId = registration.SemesterId });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminManageOfferings(int semesterId)
        {
            var offerings = await _context.SemesterOfferedCourses
                .Where(o => o.SemesterId == semesterId)
                .Include(o => o.Course)
                .Include(o => o.Department)
                .Include(o => o.Schedules)
                .ToListAsync();

            ViewBag.RegisteredCounts = await _context.RegistrationItems
                .Where(i => i.Registration!.SemesterId == semesterId
                    && i.Status == UniversityManagementSystem.Domain.Enums.RegistrationItemStatus.Registered)
                .GroupBy(i => i.SemesterOfferedCourseId)
                .Select(group => new { OfferingId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(group => group.OfferingId, group => group.Count);

            ViewBag.SemesterId = semesterId;
            ViewBag.SemesterName = await _context.Semesters.Where(s => s.Id == semesterId).Select(s => s.Name).FirstOrDefaultAsync();

            return View(offerings);
        }
    }
}
