using UniversityManagementSystem.Application.DTOs.Assignments;
using UniversityManagementSystem.Application.DTOs.Attendance;
using UniversityManagementSystem.Application.DTOs.Chat;
using UniversityManagementSystem.Application.DTOs.Exams;
using UniversityManagementSystem.Application.DTOs.Instructor;
using UniversityManagementSystem.Application.DTOs.Results;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Enums;
using UniversityManagementSystem.Web.Models;
using UniversityManagementSystem.Web.Models.ViewModels;
using UniversityManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly IInstructorService _instructorService;
        private readonly ICourseService _courseService;
        private readonly IAttendanceService _attendanceService;
        private readonly IExamService _examService;
        private readonly IAssignmentService _assignmentService;
        private readonly IGradeService _gradeService;
        private readonly IUserManagementService _userService;
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IChatService _chatService;
        private readonly IFileService _fileService;
        private readonly IAuditLogService _auditLogService;
        private readonly IDashboardService _dashboardService;
        private readonly IResultService _resultService;
        private readonly ApplicationDbContext _context;

        public InstructorController(
            IInstructorService instructorService,
            ICourseService courseService,
            IAttendanceService attendanceService,
            IExamService examService,
            IAssignmentService assignmentService,
            IGradeService gradeService,
            IUserManagementService userService,
            IStudentService studentService,
            IEnrollmentService enrollmentService,
            IChatService chatService,
            IFileService fileService,
            IAuditLogService auditLogService,
            IDashboardService dashboardService,
            IResultService resultService,
            ApplicationDbContext context)
        {
            _instructorService = instructorService;
            _courseService = courseService;
            _attendanceService = attendanceService;
            _examService = examService;
            _assignmentService = assignmentService;
            _gradeService = gradeService;
            _userService = userService;
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _chatService = chatService;
            _fileService = fileService;
            _auditLogService = auditLogService;
            _dashboardService = dashboardService;
            _resultService = resultService;
            _context = context;
        }

        // === Dashboard ===
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var instructorName = string.Join(" ", new[] { instructor.FirstName, instructor.MiddleName, instructor.LastName }.Where(name => !string.IsNullOrWhiteSpace(name)));
            var model = await _dashboardService.GetInstructorDashboardAsync(instructor.Id, instructorName);
            return View(model);
        }

        // ========== Timetable ==========

        /// <summary>
        /// Instructor Timetable: read-only view of the university timetable
        /// filtered to the instructor's assigned courses.
        /// </summary>
        public async Task<IActionResult> InstructorTimetable(int? academicYearId, int? semesterId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            // Get instructor's name to match timetable entries
            var instructorName = $"{instructor.FirstName} {instructor.LastName}";

            // Load filter options
            var academicYears = await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync();
            ViewBag.AcademicYears = academicYears;

            // Auto-select current academic year if none specified
            if (!academicYearId.HasValue)
            {
                var currentYear = await _context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent);
                if (currentYear != null) academicYearId = currentYear.Id;
            }

            var semesters = await _context.Semesters
                .Where(s => s.AcademicYearId == (academicYearId ?? 0))
                .OrderBy(s => s.Name)
                .ToListAsync();
            ViewBag.Semesters = semesters;

            // Auto-select first semester if none specified
            if (!semesterId.HasValue && semesters.Any())
            {
                semesterId = semesters.First().Id;
            }

            ViewBag.SelectedAcademicYearId = academicYearId;
            ViewBag.SelectedSemesterId = semesterId;

            // Get timetable entries for this instructor
            var query = _context.TimetableEntries
                .Include(t => t.AcademicYear)
                .Include(t => t.Semester)
                .Include(t => t.Grade)
                .Include(t => t.Section)
                .Include(t => t.Period)
                .AsQueryable();

            if (academicYearId.HasValue)
                query = query.Where(t => t.AcademicYearId == academicYearId.Value);
            if (semesterId.HasValue)
                query = query.Where(t => t.SemesterId == semesterId.Value);

            // Filter by instructor name (case-insensitive partial match)
            var entries = await query
                .Where(t => t.Instructor != null &&
                    t.Instructor.ToLower().Contains(instructorName.ToLower()))
                .OrderBy(t => t.Day)
                .ThenBy(t => t.Period != null ? t.Period.DisplayOrder : 0)
                .ToListAsync();

            // If no entries matched by name, also try matching by courses the instructor teaches
            if (!entries.Any())
            {
                var courseTitles = (await _courseService.GetCoursesByInstructorAsync(instructor.Id))
                    .Select(c => c.Title.ToLower())
                    .ToList();

                entries = await query
                    .Where(t => courseTitles.Any(ct => t.Subject.ToLower().Contains(ct)))
                    .OrderBy(t => t.Day)
                    .ThenBy(t => t.Period != null ? t.Period.DisplayOrder : 0)
                    .ToListAsync();
            }

            // Check if any timetable entries exist at all (admin has created a timetable)
            var hasAnyTimetable = await _context.TimetableEntries.AnyAsync();
            ViewBag.HasAnyTimetable = hasAnyTimetable;

            // Get periods for the grid header
            var periods = await _context.Periods
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            ViewBag.Periods = periods;

            return View(entries);
        }

        // ========== Courses ==========
        public async Task<IActionResult> Courses()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            return View(courses);
        }

        public async Task<IActionResult> Students(int courseId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var ownsCourse = (await _courseService.GetCoursesByInstructorAsync(instructor.Id)).Any(c => c.Id == courseId);
            if (!ownsCourse) return Forbid();

            // Only show approved enrollments to instructors
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
            var approvedEnrollments = enrollments.Where(e => e.Status == "Approved").ToList();
            var registeredStudents = await _context.RegistrationItems
                .Where(i => i.SemesterOfferedCourse!.CourseId == courseId
                    && i.Status == RegistrationItemStatus.Registered
                    && i.Registration!.Status == RegistrationStatus.Approved)
                .Include(i => i.Registration)
                    .ThenInclude(r => r!.Student)
                .Include(i => i.SemesterOfferedCourse)
                    .ThenInclude(o => o!.Course)
                .ToListAsync();
            approvedEnrollments.AddRange(registeredStudents
                .Where(i => i.Registration?.Student != null
                    && !approvedEnrollments.Any(e => e.StudentId == i.Registration.Student!.StudentId))
                .GroupBy(i => i.Registration!.Student!.StudentId)
                .Select(group => group.OrderByDescending(i => i.Registration!.CreatedAt).First())
                .Select(i => new UniversityManagementSystem.Application.DTOs.Enrollment.EnrollmentListDto
                {
                    Id = i.Id,
                    StudentFullName = string.Join(" ", new[]
                    {
                        i.Registration!.Student!.FirstName,
                        i.Registration.Student.MiddleName,
                        i.Registration.Student.LastName
                    }.Where(name => !string.IsNullOrWhiteSpace(name))),
                    StudentId = i.Registration!.Student!.StudentId,
                    CourseId = courseId,
                    CourseTitle = i.SemesterOfferedCourse!.Course!.Title,
                    CourseCode = i.SemesterOfferedCourse.Course.Code,
                    Status = "Approved",
                    EnrollmentDate = i.Registration.CreatedAt,
                    ApprovalDate = i.Registration.ReviewedAt
                }));
            ViewBag.CourseId = courseId;
            return View(approvedEnrollments);
        }

        // ========== Attendance ==========

        /// <summary>
        /// Attendance overview: shows every course the instructor teaches with its
        /// attendance status, today's record state and quick links to mark/history.
        /// </summary>
        public async Task<IActionResult> Attendance()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            var model = new List<Models.InstructorAttendanceViewModel>();

            foreach (var c in courses)
            {
                var records = (await _attendanceService.GetAttendanceByCourseIdAsync(c.Id)).ToList();
                var today = records.Where(r => r.Date.Date == DateTime.Today.Date).ToList();
                model.Add(new Models.InstructorAttendanceViewModel
                {
                    CourseId = c.Id,
                    CourseTitle = c.Title,
                    CourseCode = c.Code,
                    TotalRecords = records.Count,
                    TodayRecords = today.Count,
                    PresentCount = records.Count(r => r.Status == AttendanceStatus.Present),
                    AbsentCount = records.Count(r => r.Status == AttendanceStatus.Absent),
                    LateCount = records.Count(r => r.Status == AttendanceStatus.Late),
                    ExcusedCount = records.Count(r => r.Status == AttendanceStatus.Excused)
                });
            }

            return View(model);
        }

        public async Task<IActionResult> MarkAttendance(int courseId, DateTime? date)
        {
            if (!date.HasValue) date = DateTime.Today;
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            var courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            var course = courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction(nameof(Dashboard));
            }
            ViewBag.CourseName = course.Title;
            ViewBag.Courses = courses;

            // Load existing attendance records for this course/date so saved statuses are shown
            var existingAttendance = await _attendanceService.GetAttendanceByCourseIdAsync(courseId);
            var existingForDate = existingAttendance.Where(a => a.Date.Date == date.Value.Date).ToList();

            var students = await _studentService.GetStudentsByCourseAsync(courseId);
            var model = new AttendanceMarkViewModel
            {
                CourseId = courseId,
                Date = date.Value,
                Students = students.Select(s =>
                {
                    // Check if this student already has an attendance record for this date
                    var existing = existingForDate.FirstOrDefault(a => a.StudentId == s.Id);
                    return new AttendanceStudentViewModel
                    {
                        StudentId = s.Id,
                        StudentName = s.FullName,
                        Status = existing != null ? existing.Status : AttendanceStatus.Present,
                        Remarks = existing?.Remarks ?? string.Empty
                    };
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(AttendanceMarkViewModel model)
        {
            // Always redirect after POST to avoid broken model state — never return View(model)
            var user = await _userService.GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Please fix: " + string.Join(", ", errors);
                return RedirectToAction(nameof(MarkAttendance), new { courseId = model.CourseId, date = model.Date.ToString("yyyy-MM-dd") });
            }

            if (model.Students == null || !model.Students.Any())
            {
                TempData["Error"] = "No student data received. Please try again.";
                return RedirectToAction(nameof(MarkAttendance), new { courseId = model.CourseId, date = model.Date.ToString("yyyy-MM-dd") });
            }

            try
            {
                var dto = new BulkAttendanceDto
                {
                    CourseId = model.CourseId,
                    Date = model.Date,
                    Attendances = model.Students.Select(s => new AttendanceCreateDto
                    {
                        StudentId = s.StudentId,
                        CourseId = model.CourseId,
                        Date = model.Date,
                        Status = s.Status,
                        Remarks = s.Remarks ?? string.Empty
                    }).ToList()
                };
                await _attendanceService.MarkBulkAttendanceAsync(dto);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "MarkAttendance", "Attendance", model.CourseId, $"Marked attendance for course {model.CourseId} on {model.Date.ToShortDateString()}");
                TempData["Success"] = $"Attendance saved for {model.Date:MMM dd, yyyy}. All students with their correct statuses are shown below.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to save attendance: {ex.Message}";
            }

            return RedirectToAction(nameof(MarkAttendance), new { courseId = model.CourseId, date = model.Date.ToString("yyyy-MM-dd") });
        }

        public async Task<IActionResult> AttendanceHistory(int courseId, DateTime? date = null)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var courses = (await _courseService.GetCoursesByInstructorAsync(instructor.Id)).ToList();
            if (!courses.Any())
            {
                TempData["Error"] = "You have no courses assigned.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Honor the requested course; fall back to the first only when it is invalid/missing.
            var course = courses.FirstOrDefault(c => c.Id == courseId) ?? courses.First();
            courseId = course.Id;

            ViewBag.CourseName = course.Title;
            ViewBag.Courses = courses;
            ViewBag.CourseId = courseId;
            ViewBag.SelectedDate = date;

            var history = (await _attendanceService.GetAttendanceByCourseIdAsync(courseId)).ToList();
            if (date.HasValue)
            {
                history = history.Where(a => a.Date.Date == date.Value.Date).ToList();
            }
            return View(history);
        }

        public async Task<IActionResult> EditAttendance(int id)
        {
            var att = await _attendanceService.GetAttendanceByIdAsync(id);
            if (att == null) return NotFound();
            return View(att);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttendance(AttendanceUpdateDto dto)
        {
            await _attendanceService.UpdateAttendanceAsync(dto);
            var user = await _userService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "EditAttendance", "Attendance", dto.Id, $"Updated attendance ID {dto.Id}");
            TempData["Success"] = "Attendance updated.";
            return RedirectToAction(nameof(AttendanceHistory), new { courseId = dto.CourseId });
        }

        // ========== My Students (across all taught courses) ==========

        /// <summary>
        /// Lists approved students across every course the instructor teaches,
        /// with an optional course filter.
        /// </summary>
        public async Task<IActionResult> MyStudents(int? courseId = null)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var courses = (await _courseService.GetCoursesByInstructorAsync(instructor.Id)).ToList();
            ViewBag.Courses = courses;

            var all = new List<UniversityManagementSystem.Application.DTOs.Enrollment.EnrollmentListDto>();
            foreach (var c in courses)
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(c.Id);
                all.AddRange(enrollments.Where(e => string.Equals(e.Status, "Approved", System.StringComparison.OrdinalIgnoreCase)));
            }
            var courseIds = courses.Select(c => c.Id).ToList();
            var registeredStudents = await _context.RegistrationItems
                .Where(i => courseIds.Contains(i.SemesterOfferedCourse!.CourseId)
                    && i.Status == RegistrationItemStatus.Registered
                    && i.Registration!.Status == RegistrationStatus.Approved)
                .Include(i => i.Registration)
                    .ThenInclude(r => r!.Student)
                .Include(i => i.SemesterOfferedCourse)
                    .ThenInclude(o => o!.Course)
                .ToListAsync();
            all.AddRange(registeredStudents
                .Where(i => i.Registration?.Student != null
                    && !all.Any(e => e.StudentId == i.Registration.Student!.StudentId && e.CourseId == i.SemesterOfferedCourse!.CourseId))
                .GroupBy(i => new { i.Registration!.Student!.StudentId, i.SemesterOfferedCourse!.CourseId })
                .Select(group => group.OrderByDescending(i => i.Registration!.CreatedAt).First())
                .Select(i => new UniversityManagementSystem.Application.DTOs.Enrollment.EnrollmentListDto
                {
                    Id = i.Id,
                    StudentFullName = string.Join(" ", new[]
                    {
                        i.Registration!.Student!.FirstName,
                        i.Registration.Student.MiddleName,
                        i.Registration.Student.LastName
                    }.Where(name => !string.IsNullOrWhiteSpace(name))),
                    StudentId = i.Registration!.Student!.StudentId,
                    CourseId = i.SemesterOfferedCourse!.CourseId,
                    CourseTitle = i.SemesterOfferedCourse.Course!.Title,
                    CourseCode = i.SemesterOfferedCourse.Course.Code,
                    Status = "Approved",
                    EnrollmentDate = i.Registration.CreatedAt,
                    ApprovalDate = i.Registration.ReviewedAt
                }));

            IEnumerable<UniversityManagementSystem.Application.DTOs.Enrollment.EnrollmentListDto> model = all;
            if (courseId.HasValue && courseId.Value > 0 && courses.Any(c => c.Id == courseId.Value))
            {
                model = all.Where(e => e.CourseId == courseId.Value);
                ViewBag.SelectedCourseId = courseId.Value;
            }
            else
            {
                ViewBag.SelectedCourseId = 0;
            }

            ViewBag.TotalStudents = all.Select(e => e.StudentId).Distinct().Count();
            ViewBag.CourseCount = courses.Count;
            return View(model);
        }

        // ========== Exams ==========

        public async Task<IActionResult> Exams()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            var exams = await _examService.GetExamsByInstructorIdAsync(instructor.Id);
            return View(exams);
        }		public async Task<IActionResult> CreateExam(int? courseId = null)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            var courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            ViewBag.Courses = courses;
            var dto = new ExamDto();
            if (courseId.HasValue && courses.Any(c => c.Id == courseId.Value))
                dto.CourseId = courseId.Value;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(ExamDto dto)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
                return View(dto);
            }
            if (dto.EndDateTime <= dto.StartDateTime)
            {
                ModelState.AddModelError(nameof(dto.EndDateTime), "End date & time must be after the start date & time.");
                ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
                return View(dto);
            }

            if (dto.AttachmentFile != null)
            {
                dto.AttachmentPath = await _fileService.SaveFileAsync(dto.AttachmentFile, "exams");
            }

            dto.InstructorId = instructor.Id;
            // A newly created exam is posted immediately so enrolled students can see it.
            var createdExam = await _examService.CreateExamAsync(dto);
            await _examService.PublishExamAsync(createdExam.Id);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "Create", "Exam", null, $"Created exam '{dto.Title}'");
            TempData["Success"] = "Exam created! Now add questions to it.";
            return RedirectToAction(nameof(ManageQuestions), new { examId = createdExam.Id });
        }

        public async Task<IActionResult> EditExam(int id)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null) return NotFound();
            ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExam(ExamDto dto)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid && dto.EndDateTime > dto.StartDateTime)
            {
                if (dto.AttachmentFile != null)
                {
                    dto.AttachmentPath = await _fileService.SaveFileAsync(dto.AttachmentFile, "exams");
                }

                await _examService.UpdateExamAsync(dto);
                TempData["Success"] = "Exam updated.";
                return RedirectToAction(nameof(Exams));
            }
            if (ModelState.IsValid && dto.EndDateTime <= dto.StartDateTime)
                ModelState.AddModelError(nameof(dto.EndDateTime), "End date & time must be after the start date & time.");

            ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            var examForView = await _examService.GetExamByIdAsync(dto.Id);
            ViewBag.ExamAttachmentPath = examForView?.AttachmentPath;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExam(int id)
        {
            try
            {
                await _examService.DeleteExamAsync(id);
                var user = await _userService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "Delete", "Exam", id, $"Deleted exam ID {id}");
                TempData["Success"] = "Exam deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete exam: {ex.Message}";
            }
            return RedirectToAction(nameof(Exams));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishExam(int id)
        {
            try
            {
                await _examService.PublishExamAsync(id);
                var user = await _userService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "Publish", "Exam", id, $"Published exam ID {id}");
                TempData["Success"] = "Exam published.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to publish exam: {ex.Message}";
            }
            return RedirectToAction(nameof(Exams));
        }        // ========== Question Management ==========

        [HttpGet]
        public async Task<IActionResult> ManageQuestions(int examId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var exam = await _examService.GetExamWithQuestionsAsync(examId);
            if (exam == null) return NotFound();

            ViewBag.Exam = exam;
            ViewBag.ExamId = examId;
            return View(exam.Questions ?? new List<QuestionDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(QuestionDto dto)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            // Validate MCQ options
            if (dto.Type == ExamType.MCQ && (dto.Options == null || dto.Options.Count < 2))
            {
                TempData["Error"] = "MCQ questions must have at least 2 options.";
                return RedirectToAction(nameof(ManageQuestions), new { examId = dto.ExamId });
            }

            // Ensure at least one option is marked correct for MCQ
            if (dto.Type == ExamType.MCQ && !dto.Options.Any(o => o.IsCorrect))
            {
                // Default to first option if none marked as correct
                var firstOption = dto.Options.FirstOrDefault();
                if (firstOption != null)
                    firstOption.IsCorrect = true;
            }

            // Set the correct answer text from the correct option for MCQ
            if (dto.Type == ExamType.MCQ)
            {
                var correctOption = dto.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null)
                    dto.CorrectAnswer = correctOption.OptionText;
            }

            try
            {
                await _examService.AddQuestionAsync(dto);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "AddQuestion", "Question", null, $"Added question to exam ID {dto.ExamId}");
                TempData["Success"] = "Question added successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to add question: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageQuestions), new { examId = dto.ExamId });
        }

        [HttpGet]
        public async Task<IActionResult> EditQuestion(int id)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var question = await _examService.GetQuestionWithOptionsAsync(id);
            if (question == null) return NotFound();

            var exam = await _examService.GetExamByIdAsync(question.ExamId);
            ViewBag.Exam = exam;
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(QuestionDto dto)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            // For MCQ, set correct answer from options
            if (dto.Type == ExamType.MCQ && dto.Options != null)
            {
                var correctOption = dto.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null)
                    dto.CorrectAnswer = correctOption.OptionText;
            }

            try
            {
                await _examService.UpdateQuestionAsync(dto);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "EditQuestion", "Question", dto.Id, $"Updated question ID {dto.Id}");
                TempData["Success"] = "Question updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update question: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageQuestions), new { examId = dto.ExamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id, int examId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            try
            {
                await _examService.DeleteQuestionAsync(id);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "DeleteQuestion", "Question", id, $"Deleted question ID {id} from exam {examId}");
                TempData["Success"] = "Question deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete question: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageQuestions), new { examId });
        }

        [HttpGet]
        public async Task<JsonResult> GetQuestionOptions(int questionId)
        {
            try
            {
                var question = await _examService.GetQuestionWithOptionsAsync(questionId);
                if (question?.Options == null)
                    return Json(new List<object>());

                var options = question.Options.Select(o => new
                {
                    id = o.Id,
                    optionText = o.OptionText,
                    isCorrect = o.IsCorrect
                }).ToList();

                return Json(options);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        public async Task<IActionResult> GradeExam(int examId)
        {
            var exam = await _examService.GetExamWithQuestionsAsync(examId);
            var submissions = await _examService.GetSubmissionsForExamAsync(examId);
            ViewBag.Exam = exam;
            return View(submissions);
        }

        [HttpPost]
        public async Task<IActionResult> GradeExam(int examId, int studentId, Dictionary<int, string> answers)
        {
            await _examService.GradeExamAsync(examId, studentId, answers);
            var user = await _userService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "GradeExam", "ExamResult", null, $"Graded exam {examId} for student {studentId}");
            TempData["Success"] = "Exam graded.";
            return RedirectToAction(nameof(Exams));
        }

        // Manual Essay Grading
        public async Task<IActionResult> GradeEssays(int examId, int studentId)
        {
            var exam = await _examService.GetExamWithQuestionsAsync(examId);
            var answers = await _examService.GetStudentAnswersForGradingAsync(examId, studentId);
            var result = await _examService.GetResultByStudentAndExamAsync(studentId, examId);
            ViewBag.Exam = exam;
            ViewBag.Result = result;
            return View(answers);
        }

        [HttpPost]
        public async Task<IActionResult> GradeEssays(int examId, int studentId, Dictionary<int, int> scores)
        {
            var user = await _userService.GetCurrentUserAsync();
            await _examService.ManualGradeEssayAsync(examId, studentId, scores);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "ManualGradeEssay", "ExamResult", null, $"Manually graded essay questions for exam {examId}, student {studentId}");
            TempData["Success"] = "Essay answers graded successfully.";
            return RedirectToAction(nameof(GradeExam), new { examId });
        }

        public async Task<IActionResult> UpdateExamResult(int examResultId)
        {
            var result = await _examService.GetResultByIdAsync(examResultId);
            if (result == null) return NotFound();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExamResult(int examResultId, int totalScore, string remarks)
        {
            var user = await _userService.GetCurrentUserAsync();
            var updated = await _examService.UpdateManualExamResultAsync(examResultId, totalScore, remarks);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "UpdateExamResult", "ExamResult", examResultId, $"Updated exam result {examResultId}: score {totalScore}");
            TempData["Success"] = "Exam result updated.";
            return RedirectToAction(nameof(GradeExam), new { examId = updated.ExamId });
        }

        // ========== Assignments ==========

        public async Task<IActionResult> Assignments()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            var assignments = await _assignmentService.GetAssignmentsByInstructorIdAsync(instructor.Id);
            return View(assignments);
        }        public async Task<IActionResult> CreateAssignment(int? courseId = null)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null)
            {
                TempData["Error"] = "Instructor profile not found. Please contact an administrator.";
                return RedirectToAction("Index", "Home");
            }
            var courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            ViewBag.Courses = courses;
            var dto = new AssignmentCreateDto();
            if (courseId.HasValue && courses.Any(c => c.Id == courseId.Value))
                dto.CourseId = courseId.Value;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignment(AssignmentCreateDto dto)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
                return View(dto);
            }
            try
            {
                var createdAssignment = await _assignmentService.CreateAssignmentAsync(dto, instructor.Id);
                await _assignmentService.PublishAssignmentAsync(createdAssignment.Id);
                TempData["Success"] = "Assignment created! Now add questions to it.";
                return RedirectToAction(nameof(ManageAssignmentQuestions), new { assignmentId = createdAssignment.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            return View(dto);
        }

        public async Task<IActionResult> EditAssignment(int id)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            var assignmentDto = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignmentDto == null) return NotFound();
            var assignment = new AssignmentUpdateDto
            {
                Id = assignmentDto.Id,
                Title = assignmentDto.Title,
                Description = assignmentDto.Description,
                CourseId = assignmentDto.CourseId,
                Deadline = assignmentDto.Deadline,
                TotalMarks = assignmentDto.TotalMarks,
                AllowLateSubmission = assignmentDto.AllowLateSubmission,
                LateSubmissionPenalty = assignmentDto.LateSubmissionPenalty
            };
            ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> EditAssignment(AssignmentUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _assignmentService.UpdateAssignmentAsync(dto);
                    TempData["Success"] = "Assignment updated.";
                    return RedirectToAction(nameof(Assignments));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            ViewBag.Courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishAssignment(int id)
        {
            try
            {
                await _assignmentService.PublishAssignmentAsync(id);
                var user = await _userService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "Publish", "Assignment", id, $"Published assignment ID {id}");
                TempData["Success"] = "Assignment published.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to publish assignment: {ex.Message}";
            }
            return RedirectToAction(nameof(Assignments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            try
            {
                await _assignmentService.DeleteAssignmentAsync(id);
                var user = await _userService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "Delete", "Assignment", id, $"Deleted assignment ID {id}");
                TempData["Success"] = "Assignment deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete assignment: {ex.Message}";
            }
            return RedirectToAction(nameof(Assignments));
        }

        public async Task<IActionResult> ViewSubmissions(int assignmentId)
        {
            var submissions = await _assignmentService.GetSubmissionsByAssignmentIdAsync(assignmentId);
            return View(submissions);
        }

        public async Task<IActionResult> GradeSubmission(int submissionId)
        {
            var submission = await _assignmentService.GetSubmissionByIdAsync(submissionId);
            if (submission == null) return NotFound();
            return View(submission);
        }

        [HttpPost]
        public async Task<IActionResult> GradeSubmission(SubmissionGradeDto dto)
        {
            if (ModelState.IsValid)
            {
                await _assignmentService.GradeSubmissionAsync(dto);
                var user = await _userService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "GradeSubmission", "Submission", dto.SubmissionId, $"Graded submission {dto.SubmissionId} with score {dto.Grade}");
                TempData["Success"] = "Submission graded.";
                return RedirectToAction(nameof(ViewSubmissions), new { assignmentId = dto.AssignmentId });
            }
            return View(dto);
        }

        // ========== Assignment Questions ==========

        [HttpGet]
        public async Task<IActionResult> ManageAssignmentQuestions(int assignmentId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var assignment = await _assignmentService.GetAssignmentByIdAsync(assignmentId);
            if (assignment == null) return NotFound();

            ViewBag.Assignment = assignment;
            ViewBag.AssignmentId = assignmentId;
            return View(assignment.Questions ?? new List<AssignmentQuestionDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssignmentQuestion(AssignmentQuestionDto dto)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            // Validate MCQ options
            if (dto.Type == ExamType.MCQ && (dto.Options == null || dto.Options.Count < 2))
            {
                TempData["Error"] = "MCQ questions must have at least 2 options.";
                return RedirectToAction(nameof(ManageAssignmentQuestions), new { assignmentId = dto.AssignmentId });
            }

            if (dto.Type == ExamType.MCQ && !dto.Options.Any(o => o.IsCorrect))
            {
                var firstOption = dto.Options.FirstOrDefault();
                if (firstOption != null)
                    firstOption.IsCorrect = true;
            }

            if (dto.Type == ExamType.MCQ)
            {
                var correctOption = dto.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null)
                    dto.CorrectAnswer = correctOption.OptionText;
            }

            try
            {
                await _assignmentService.AddAssignmentQuestionAsync(dto);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "AddAssignmentQuestion", "AssignmentQuestion", null, $"Added question to assignment ID {dto.AssignmentId}");
                TempData["Success"] = "Assignment question added successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to add question: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageAssignmentQuestions), new { assignmentId = dto.AssignmentId });
        }

        [HttpGet]
        public async Task<JsonResult> GetAssignmentQuestionOptions(int questionId)
        {
            try
            {
                var question = await _assignmentService.GetAssignmentQuestionWithOptionsAsync(questionId);
                if (question?.Options == null)
                    return Json(new List<object>());

                var options = question.Options.Select(o => new
                {
                    id = o.Id,
                    optionText = o.OptionText,
                    isCorrect = o.IsCorrect
                }).ToList();

                return Json(options);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignmentQuestion(AssignmentQuestionDto dto)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            if (dto.Type == ExamType.MCQ && dto.Options != null)
            {
                var correctOption = dto.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null)
                    dto.CorrectAnswer = correctOption.OptionText;
            }

            try
            {
                await _assignmentService.UpdateAssignmentQuestionAsync(dto);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "EditAssignmentQuestion", "AssignmentQuestion", dto.Id, $"Updated assignment question ID {dto.Id}");
                TempData["Success"] = "Assignment question updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update question: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageAssignmentQuestions), new { assignmentId = dto.AssignmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssignmentQuestion(int id, int assignmentId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            try
            {
                await _assignmentService.DeleteAssignmentQuestionAsync(id);
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "DeleteAssignmentQuestion", "AssignmentQuestion", id, $"Deleted assignment question ID {id} from assignment {assignmentId}");
                TempData["Success"] = "Assignment question deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete question: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageAssignmentQuestions), new { assignmentId });
        }

        // ========== Assignment Grading ==========

        public async Task<IActionResult> GradeAssignmentSubmissions(int assignmentId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var assignment = await _assignmentService.GetAssignmentByIdAsync(assignmentId);
            var results = await _assignmentService.GetAssignmentResultsByAssignmentIdAsync(assignmentId);
            ViewBag.Assignment = assignment;
            return View(results);
        }

        public async Task<IActionResult> GradeAssignmentEssays(int assignmentId, int studentId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var assignment = await _assignmentService.GetAssignmentByIdAsync(assignmentId);
            var answers = await _assignmentService.GetStudentAssignmentAnswersForGradingAsync(assignmentId, studentId);
            var result = await _assignmentService.GetAssignmentResultByStudentAndAssignmentAsync(studentId, assignmentId);
            ViewBag.Assignment = assignment;
            ViewBag.Result = result;
            return View(answers);
        }

        [HttpPost]
        public async Task<IActionResult> GradeAssignmentEssays(int assignmentId, int studentId, Dictionary<int, int> scores)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            await _assignmentService.ManualGradeAssignmentEssayAsync(assignmentId, studentId, scores);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "GradeAssignmentEssays", "AssignmentResult", null, $"Graded assignment essays for assignment {assignmentId}, student {studentId}");
            TempData["Success"] = "Essay answers graded successfully.";
            return RedirectToAction(nameof(GradeAssignmentSubmissions), new { assignmentId });
        }

        public async Task<IActionResult> UpdateAssignmentResult(int resultId)
        {
            var result = await _assignmentService.GetAssignmentResultByIdAsync(resultId);
            if (result == null) return NotFound();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAssignmentResult(int resultId, int totalScore, string remarks)
        {
            var user = await _userService.GetCurrentUserAsync();
            var updated = await _assignmentService.UpdateAssignmentResultAsync(resultId, totalScore, remarks);
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "UpdateAssignmentResult", "AssignmentResult", resultId, $"Updated assignment result {resultId}: score {totalScore}");
            TempData["Success"] = "Assignment result updated.";
            return RedirectToAction(nameof(GradeAssignmentSubmissions), new { assignmentId = updated.AssignmentId });
        }

        // ========== Grades & Results ==========

        /// <summary>
        /// Grades &amp; Results: table of every course the instructor teaches with its
        /// result status, plus the Assessment button that opens the marks entry board.
        /// </summary>
        public async Task<IActionResult> Grades()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");
            var model = await _resultService.GetInstructorCourseResultsAsync(instructor.Id);
            return View(model);
        }

        /// <summary>
        /// Dynamic marks entry board for one course: rows are the enrolled students,
        /// columns are the assessment components the instructor can add dynamically.
        /// </summary>
        public async Task<IActionResult> AssessmentBoard(int courseId)
        {
            if (!await CanManageCourseAsync(courseId)) return Forbid();
            var model = await _resultService.GetCourseAssessmentBoardAsync(courseId);
            return View(model);
        }

        /// <summary>
        /// Saves the current board as a draft (invisible to students).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAssessmentMarks([FromBody] SaveCourseAssessmentDto dto)
        {
            if (dto == null || dto.CourseId <= 0)
                return Json(new { success = false, message = "Invalid request." });
            if (!await CanManageCourseAsync(dto.CourseId))
                return Json(new { success = false, message = "You do not have permission to manage this course." });

            try
            {
                await _resultService.SaveDraftAsync(dto.CourseId, dto);
                var user = await _userService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "SaveDraft", "CourseAssessment", dto.CourseId, $"Saved assessment marks draft for course {dto.CourseId}");
                return Json(new { success = true, message = "Marks saved as draft. Students cannot see them until you send the results." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Saves the board and publishes (sends) the results to all graded students.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAssessmentResults([FromBody] SaveCourseAssessmentDto dto)
        {
            if (dto == null || dto.CourseId <= 0)
                return Json(new { success = false, message = "Invalid request." });
            if (!await CanManageCourseAsync(dto.CourseId))
                return Json(new { success = false, message = "You do not have permission to manage this course." });

            try
            {
                var published = await _resultService.PublishResultsAsync(dto.CourseId, dto);
                var user = await _userService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Instructor", "PublishResults", "CourseResult", dto.CourseId, $"Sent results for course {dto.CourseId} to {published} student(s)");
                return Json(new { success = true, message = $"Results sent to {published} student(s). They can now see their grades." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<bool> CanManageCourseAsync(int courseId)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null) return false;
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return false;
            var courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            return courses.Any(c => c.Id == courseId);
        }

        // ========== My Course Assignments ==========

        /// <summary>
        /// My Course Assignments: shows every course assigned to this instructor
        /// as a professional card grid with quick-access actions.
        /// </summary>
        public async Task<IActionResult> MyAssignments()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            var courses = (await _courseService.GetCoursesByInstructorAsync(instructor.Id)).ToList();
            var allAssignments = (await _assignmentService.GetAssignmentsByInstructorIdAsync(instructor.Id)).ToList();
            var allExams = (await _examService.GetExamsByInstructorIdAsync(instructor.Id)).ToList();

            var model = new List<Models.ViewModels.InstructorCourseAssignmentViewModel>();
            foreach (var c in courses)
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(c.Id);
                var approvedEnrollments = enrollments.Where(e => e.Status == "Approved").ToList();
                var approvedStudentNumbers = approvedEnrollments.Select(e => e.StudentId).Distinct().ToList();
                var registeredCount = await _context.RegistrationItems
                    .Where(i => i.SemesterOfferedCourse!.CourseId == c.Id
                        && i.Status == RegistrationItemStatus.Registered
                        && i.Registration!.Status == RegistrationStatus.Approved
                        && !approvedStudentNumbers.Contains(i.Registration.Student!.StudentId))
                    .Select(i => i.Registration!.Student!.StudentId)
                    .Distinct()
                    .CountAsync();
                var courseAssignments = allAssignments.Where(a => a.CourseId == c.Id).ToList();
                var courseExams = allExams.Where(e => e.CourseId == c.Id).ToList();
                var totalSubmissions = courseAssignments.Sum(a => a.SubmissionsCount);

                model.Add(new Models.ViewModels.InstructorCourseAssignmentViewModel
                {
                    CourseId = c.Id,
                    CourseTitle = c.Title,
                    CourseCode = c.Code,
                    DepartmentName = c.DepartmentName,
                    Credits = c.Credits,
                    EnrollmentCount = approvedEnrollments.Select(e => e.StudentId).Distinct().Count() + registeredCount,
                    AssignmentCount = courseAssignments.Count,
                    ExamCount = courseExams.Count,
                    PendingSubmissions = totalSubmissions,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                });
            }

            return View(model);
        }

        // ========== Course Class Management (opened from MyAssignments) ==========

        /// <summary>
        /// Course class management hub: shows course info and quick links to
        /// Students, Assignments, Exams, Attendance, and Grades for one course.
        /// </summary>
        public async Task<IActionResult> CourseClass(int courseId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            if (instructor == null) return RedirectToAction("Index", "Home");

            // Verify the instructor owns this course
            var courses = await _courseService.GetCoursesByInstructorAsync(instructor.Id);
            var course = courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found or you do not have access.";
                return RedirectToAction(nameof(MyAssignments));
            }

            // Gather stats
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
            var approvedStudentIds = enrollments.Where(e => e.Status == "Approved").Select(e => e.StudentId).ToHashSet();
            var registeredStudentIds = await _context.RegistrationItems
                .Where(i => i.SemesterOfferedCourse!.CourseId == courseId
                    && i.Status == RegistrationItemStatus.Registered
                    && i.Registration!.Status == RegistrationStatus.Approved)
                .Select(i => i.Registration!.Student!.StudentId)
                .Distinct()
                .ToListAsync();
            var approvedCount = approvedStudentIds.Union(registeredStudentIds).Count();
            var allAssignments = (await _assignmentService.GetAssignmentsByInstructorIdAsync(instructor.Id))
                .Where(a => a.CourseId == courseId).ToList();
            var allExams = (await _examService.GetExamsByInstructorIdAsync(instructor.Id))
                .Where(e => e.CourseId == courseId).ToList();

            ViewBag.CourseId = course.Id;
            ViewBag.CourseTitle = course.Title;
            ViewBag.CourseCode = course.Code;
            ViewBag.DepartmentName = course.DepartmentName;
            ViewBag.Credits = course.Credits;
            ViewBag.StudentCount = approvedCount;
            ViewBag.AssignmentCount = allAssignments.Count;
            ViewBag.ExamCount = allExams.Count;
            ViewBag.StartDate = course.StartDate;
            ViewBag.EndDate = course.EndDate;

            return View();
        }

        // ========== Profile ==========
        public async Task<IActionResult> Profile()
        {
            var user = await _userService.GetCurrentUserAsync();
            var instructor = await _instructorService.GetInstructorByUserIdAsync(user.Id);
            return View(instructor);
        }
    }
}
