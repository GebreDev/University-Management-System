using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TimetableController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserManagementService _userManagementService;

        public TimetableController(ApplicationDbContext context, IAuditLogService auditLogService, IUserManagementService userManagementService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userManagementService = userManagementService;
        }

        // ===== Helper: write an audit entry attributed to the signed-in admin =====
        private async Task LogAdminActionAsync(string action, string entity, int? entityId, string details)
        {
            ApplicationUser? actor = null;
            try { actor = await _userManagementService.GetCurrentUserAsync(); }
            catch { /* user context unavailable – fall back below */ }

            await _auditLogService.LogAsync(
                actor?.Id,
                actor != null ? $"{actor.FirstName} {actor.LastName}" : "Admin",
                "Admin",
                action,
                entity,
                entityId,
                details);
        }

        // ===== Show Timetable (main view with cascading dropdowns + grid) =====
        public async Task<IActionResult> ShowTimetable()
        {
            ViewBag.AcademicYears = await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync();
            ViewBag.Grades = await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync();
            ViewBag.Periods = await _context.Periods.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder).ToListAsync();
            return View();
        }

        // ===== Get timetable entries by filters (AJAX) =====
        [HttpGet]
        public async Task<IActionResult> GetTimetable(int academicYearId, int semesterId, int gradeId, int sectionId)
        {
            var entries = await _context.TimetableEntries
                .Include(t => t.AcademicYear)
                .Include(t => t.Semester)
                .Include(t => t.Grade)
                .Include(t => t.Section)
                .Include(t => t.Period)
                .Where(t => t.AcademicYearId == academicYearId
                         && t.SemesterId == semesterId
                         && t.GradeId == gradeId
                         && t.SectionId == sectionId)
                .OrderBy(t => t.Day).ThenBy(t => t.Period!.DisplayOrder)
                .ToListAsync();

            return Json(entries.Select(t => new
            {
                t.Id,
                t.Day,
                PeriodName = t.Period?.Name ?? "Unknown",
                PeriodOrder = t.Period?.DisplayOrder ?? 0,
                t.Subject,
                t.Instructor,
                t.Room
            }));
        }

        // ===== AJAX: Semesters by AcademicYear =====
        [HttpGet]
        public async Task<IActionResult> GetSemesters(int academicYearId)
        {
            var semesters = await _context.Semesters
                .Where(s => s.AcademicYearId == academicYearId)
                .OrderBy(s => s.Name)
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();
            return Json(semesters);
        }

        // ===== AJAX: Sections by Grade =====
        [HttpGet]
        public async Task<IActionResult> GetSections(int gradeId)
        {
            var sections = await _context.AcademicSections
                .Where(s => s.GradeId == gradeId)
                .OrderBy(s => s.Name)
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();
            return Json(sections);
        }

        // ===== Create Timetable (form to add entry) =====
        public async Task<IActionResult> CreateTimetable(int? academicYearId, int? semesterId, int? gradeId, int? sectionId)
        {
            await LoadViewBagsForTimetable(academicYearId, semesterId, gradeId, sectionId);

            // Pass periods as list for the grid
            ViewBag.PeriodsList = await _context.Periods.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder).ToListAsync();
            ViewBag.Courses = await _context.Courses.OrderBy(c => c.Title).ToListAsync();

            // Get working days from settings
            var setting = await _context.TimetableSettings.FirstOrDefaultAsync();
            var workingDays = new List<string>();
            if (setting != null)
            {
                if (setting.MondayEnabled) workingDays.Add("Monday");
                if (setting.TuesdayEnabled) workingDays.Add("Tuesday");
                if (setting.WednesdayEnabled) workingDays.Add("Wednesday");
                if (setting.ThursdayEnabled) workingDays.Add("Thursday");
                if (setting.FridayEnabled) workingDays.Add("Friday");
                if (setting.SaturdayEnabled) workingDays.Add("Saturday");
                if (setting.SundayEnabled) workingDays.Add("Sunday");
            }
            ViewBag.WorkingDays = workingDays;

            return View(new TimetableEntry
            {
                AcademicYearId = academicYearId ?? 0,
                SemesterId = semesterId ?? 0,
                GradeId = gradeId ?? 0,
                SectionId = sectionId ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTimetable(int AcademicYearId, int SemesterId, int GradeId, int SectionId, string[] subjects, string[] instructors, string[] rooms, string[] days, int[] periodIds)
        {
            int saved = 0;
            for (int i = 0; i < subjects.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(subjects[i])) continue;

                var entry = new TimetableEntry
                {
                    AcademicYearId = AcademicYearId,
                    SemesterId = SemesterId,
                    GradeId = GradeId,
                    SectionId = SectionId,
                    Day = days[i],
                    PeriodId = periodIds[i],
                    Subject = subjects[i],
                    Instructor = instructors.Length > i ? instructors[i] : null,
                    Room = rooms.Length > i ? rooms[i] : null
                };
                _context.TimetableEntries.Add(entry);
                saved++;
            }

            if (saved > 0)
            {
                await _context.SaveChangesAsync();
                await LogAdminActionAsync("Create", "Timetable", null,
                    $"Created {saved} timetable entries for year #{AcademicYearId}, semester #{SemesterId}, year #{GradeId}, section #{SectionId}");
                TempData["Success"] = $"{saved} timetable {(saved == 1 ? "entry" : "entries")} saved successfully.";
            }
            else
            {
                TempData["Error"] = "No entries to save. Please fill in at least one subject.";
            }

            return RedirectToAction(nameof(ShowTimetable), new
            {
                academicYearId = AcademicYearId,
                semesterId = SemesterId,
                gradeId = GradeId,
                sectionId = SectionId
            });
        }

        // ===== Edit Timetable Entry =====
        public async Task<IActionResult> EditEntry(int id)
        {
            var entry = await _context.TimetableEntries.FindAsync(id);
            if (entry == null) return NotFound();

            await LoadViewBagsForTimetable(entry.AcademicYearId, entry.SemesterId, entry.GradeId, entry.SectionId);
            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEntry(int id, TimetableEntry model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                await LogAdminActionAsync("Update", "Timetable", model.Id,
                    $"Updated timetable entry ID {model.Id}: {model.Subject} on {model.Day}");
                TempData["Success"] = "Timetable entry updated successfully.";
                return RedirectToAction(nameof(ShowTimetable), new
                {
                    academicYearId = model.AcademicYearId,
                    semesterId = model.SemesterId,
                    gradeId = model.GradeId,
                    sectionId = model.SectionId
                });
            }

            await LoadViewBagsForTimetable(model.AcademicYearId, model.SemesterId, model.GradeId, model.SectionId);
            return View(model);
        }

        // ===== Delete Timetable Entry =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEntry(int id, int academicYearId, int semesterId, int gradeId, int sectionId)
        {
            var entry = await _context.TimetableEntries.FindAsync(id);
            if (entry != null)
            {
                _context.TimetableEntries.Remove(entry);
                await _context.SaveChangesAsync();
                await LogAdminActionAsync("Delete", "Timetable", id,
                    $"Deleted timetable entry ID {id}: {entry.Subject} on {entry.Day}");
                TempData["Success"] = "Timetable entry deleted successfully.";
            }
            return RedirectToAction(nameof(ShowTimetable), new { academicYearId, semesterId, gradeId, sectionId });
        }

        // ===== Periods Management =====
        public async Task<IActionResult> Periods()
        {
            var periods = await _context.Periods.OrderBy(p => p.DisplayOrder).ToListAsync();
            return View(periods);
        }

        public IActionResult CreatePeriod() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePeriod(Period model)
        {
            if (ModelState.IsValid)
            {
                _context.Periods.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Period created successfully.";
                return RedirectToAction(nameof(Periods));
            }
            return View(model);
        }

        public async Task<IActionResult> EditPeriod(int id)
        {
            var period = await _context.Periods.FindAsync(id);
            if (period == null) return NotFound();
            return View(period);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPeriod(int id, Period model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Period updated successfully.";
                return RedirectToAction(nameof(Periods));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePeriod(int id)
        {
            var period = await _context.Periods.FindAsync(id);
            if (period != null)
            {
                _context.Periods.Remove(period);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Period deleted successfully.";
            }
            return RedirectToAction(nameof(Periods));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePeriod(int id)
        {
            var period = await _context.Periods.FindAsync(id);
            if (period != null)
            {
                period.IsActive = !period.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Period {(period.IsActive ? "activated" : "deactivated")} successfully.";
            }
            return RedirectToAction(nameof(Periods));
        }

        // ===== Timetable Settings =====
        public async Task<IActionResult> TimetableSettings()
        {
            var setting = await _context.TimetableSettings.FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new TimetableSetting
                {
                    UniversityName = "University Management System",
                    DayStartTime = new TimeOnly(7, 30),
                    DayEndTime = new TimeOnly(15, 30),
                    PeriodsPerDay = 8,
                    WorkingDaysPerWeek = 6,
                    LunchBreakStart = new TimeOnly(11, 30),
                    LunchBreakEnd = new TimeOnly(12, 30),
                    LunchBreakDurationMinutes = 60
                };
                _context.TimetableSettings.Add(setting);
                await _context.SaveChangesAsync();
            }
            ViewBag.Periods = await _context.Periods.OrderBy(p => p.DisplayOrder).ToListAsync();
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TimetableSettings(TimetableSetting model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.TimetableSettings.FirstOrDefaultAsync();
                    if (existing != null)
                    {
                        existing.UniversityName = model.UniversityName;
                        existing.DayStartTime = model.DayStartTime;
                        existing.DayEndTime = model.DayEndTime;
                        existing.PeriodsPerDay = model.PeriodsPerDay;
                        existing.WorkingDaysPerWeek = model.WorkingDaysPerWeek;
                        existing.LunchBreakStart = model.LunchBreakStart;
                        existing.LunchBreakEnd = model.LunchBreakEnd;
                        existing.LunchBreakDurationMinutes = model.LunchBreakDurationMinutes;
                        existing.MondayLabel = model.MondayLabel;
                        existing.TuesdayLabel = model.TuesdayLabel;
                        existing.WednesdayLabel = model.WednesdayLabel;
                        existing.ThursdayLabel = model.ThursdayLabel;
                        existing.FridayLabel = model.FridayLabel;
                        existing.SaturdayLabel = model.SaturdayLabel;
                        existing.MondayEnabled = model.MondayEnabled;
                        existing.TuesdayEnabled = model.TuesdayEnabled;
                        existing.WednesdayEnabled = model.WednesdayEnabled;
                        existing.ThursdayEnabled = model.ThursdayEnabled;
                        existing.FridayEnabled = model.FridayEnabled;
                        existing.SaturdayEnabled = model.SaturdayEnabled;
                        existing.SundayEnabled = model.SundayEnabled;
                        existing.IsActive = model.IsActive;

                        var workingDays = new[] { model.MondayEnabled, model.TuesdayEnabled, model.WednesdayEnabled, model.ThursdayEnabled, model.FridayEnabled, model.SaturdayEnabled, model.SundayEnabled };
                        existing.WorkingDaysPerWeek = workingDays.Count(d => d);
                    }
                    else
                    {
                        _context.TimetableSettings.Add(model);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Timetable settings saved successfully.";
                    return RedirectToAction(nameof(TimetableSettings));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Unable to save timetable settings: {ex.Message}";
                    ModelState.AddModelError(string.Empty, "Unable to save timetable settings. Please try again.");
                }
            }

            ViewBag.Periods = await _context.Periods.OrderBy(p => p.DisplayOrder).ToListAsync();
            return View(model);
        }

        // ===== CRUD: Manage Academic Years =====
        public async Task<IActionResult> ManageYears()
        {
            var years = await _context.AcademicYears.OrderByDescending(y => y.IsCurrent).ThenByDescending(y => y.Name).ToListAsync();
            return View(years);
        }

        public IActionResult CreateYear() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateYear(AcademicYear model)
        {
            if (ModelState.IsValid)
            {
                _context.AcademicYears.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Academic year created.";
                return RedirectToAction(nameof(ManageYears));
            }
            return View(model);
        }

        // ===== Create Academic Year (full form with semesters) =====

        public IActionResult CreateAcademicYear()
        {
            var model = new UniversityManagementSystem.Web.Models.ViewModels.CreateAcademicYearViewModel
            {
                YearStartDate = DateTime.Today,
                YearEndDate = DateTime.Today.AddYears(1),
                Semester1StartDate = DateTime.Today,
                Semester1EndDate = DateTime.Today.AddMonths(6),
                Semester2StartDate = DateTime.Today.AddMonths(6),
                Semester2EndDate = DateTime.Today.AddYears(1)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAcademicYear(UniversityManagementSystem.Web.Models.ViewModels.CreateAcademicYearViewModel model)
        {
            if (ModelState.IsValid)
            {
                // If marking as current, unset other current years
                if (model.IsCurrent)
                {
                    var existingCurrent = await _context.AcademicYears.Where(y => y.IsCurrent).ToListAsync();
                    foreach (var y in existingCurrent)
                    {
                        y.IsCurrent = false;
                    }
                }

                var academicYear = new AcademicYear
                {
                    Name = model.YearName,
                    StartDate = model.YearStartDate,
                    EndDate = model.YearEndDate,
                    IsCurrent = model.IsCurrent
                };
                _context.AcademicYears.Add(academicYear);
                await _context.SaveChangesAsync();

                // Create Semester 1
                var sem1 = new Semester
                {
                    Name = "Semester 1",
                    AcademicYearId = academicYear.Id,
                    StartDate = model.Semester1StartDate,
                    EndDate = model.Semester1EndDate
                };
                _context.Semesters.Add(sem1);

                // Create Semester 2
                var sem2 = new Semester
                {
                    Name = "Semester 2",
                    AcademicYearId = academicYear.Id,
                    StartDate = model.Semester2StartDate,
                    EndDate = model.Semester2EndDate
                };
                _context.Semesters.Add(sem2);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Academic year and semesters created successfully.";
                return RedirectToAction(nameof(ManageYears));
            }
            return View(model);
        }

        // ===== CRUD: Manage Semesters =====
        public async Task<IActionResult> ManageSemesters(int? yearId)
        {
            var query = _context.Semesters.Include(s => s.AcademicYear).AsQueryable();
            if (yearId.HasValue) query = query.Where(s => s.AcademicYearId == yearId.Value);
            var semesters = await query.OrderBy(s => s.Name).ToListAsync();
            ViewBag.AcademicYears = await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync();
            return View(semesters);
        }

        public async Task<IActionResult> CreateSemester(int? yearId)
        {
            ViewBag.AcademicYears = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync(), "Id", "Name", yearId);
            return View(new Semester { AcademicYearId = yearId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSemester(Semester model)
        {
            if (ModelState.IsValid)
            {
                _context.Semesters.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Semester created.";
                return RedirectToAction(nameof(ManageSemesters), new { yearId = model.AcademicYearId });
            }
            ViewBag.AcademicYears = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync(), "Id", "Name", model.AcademicYearId);
            return View(model);
        }

        // ===== CRUD: Manage Grades & Sections =====
        public async Task<IActionResult> ManageGrades()
        {
            var grades = await _context.AcademicGrades.Include(g => g.Sections).OrderBy(g => g.PromotionOrder).ToListAsync();
            return View(grades);
        }

        public async Task<IActionResult> CreateGrade()
        {
            ViewBag.Instructors = await _context.Instructors.OrderBy(i => i.LastName).ToListAsync();
            return View(new AcademicGrade());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGrade(AcademicGrade model, string[] sectionNames, int?[] sectionInstructors)
        {
            if (ModelState.IsValid)
            {
                _context.AcademicGrades.Add(model);
                await _context.SaveChangesAsync();

                // Create sections
                if (sectionNames != null)
                {
                    for (int i = 0; i < sectionNames.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(sectionNames[i])) continue;
                        var section = new AcademicSection
                        {
                            Name = sectionNames[i],
                            GradeId = model.Id,
                            InstructorId = (sectionInstructors != null && i < sectionInstructors.Length) ? sectionInstructors[i] : null
                        };
                        _context.AcademicSections.Add(section);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Year created successfully.";
                return RedirectToAction(nameof(ManageGrades));
            }
            ViewBag.Instructors = await _context.Instructors.OrderBy(i => i.LastName).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> ManageSections(int gradeId)
        {
            var grade = await _context.AcademicGrades.Include(g => g.Sections).FirstOrDefaultAsync(g => g.Id == gradeId);
            if (grade == null) return NotFound();
            ViewBag.GradeName = grade.Name;
            ViewBag.GradeId = gradeId;
            return View(grade.Sections.OrderBy(s => s.Name).ToList());
        }

        public async Task<IActionResult> CreateSection(int gradeId)
        {
            ViewBag.GradeName = (await _context.AcademicGrades.FindAsync(gradeId))?.Name;
            return View(new AcademicSection { GradeId = gradeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSection(AcademicSection model)
        {
            if (ModelState.IsValid)
            {
                _context.AcademicSections.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Section created.";
                return RedirectToAction(nameof(ManageSections), new { gradeId = model.GradeId });
            }
            ViewBag.GradeName = (await _context.AcademicGrades.FindAsync(model.GradeId))?.Name;
            return View(model);
        }

        // ===== Helper: Load ViewBag data for timetable forms =====
        private async Task LoadViewBagsForTimetable(int? academicYearId, int? semesterId, int? gradeId, int? sectionId)
        {
            ViewBag.AcademicYears = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync(), "Id", "Name", academicYearId);
            ViewBag.Grades = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync(), "Id", "Name", gradeId);
            ViewBag.Semesters = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Semesters.Where(s => s.AcademicYearId == (academicYearId ?? 0)).OrderBy(s => s.Name).ToListAsync(), "Id", "Name", semesterId);
            ViewBag.Sections = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.AcademicSections.Where(s => s.GradeId == (gradeId ?? 0)).OrderBy(s => s.Name).ToListAsync(), "Id", "Name", sectionId);
            ViewBag.Periods = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Periods.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder).ToListAsync(), "Id", "Name");
        }
    }
}
