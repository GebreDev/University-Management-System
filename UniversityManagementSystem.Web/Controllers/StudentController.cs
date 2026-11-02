using UniversityManagementSystem.Application.DTOs.Assignments;
using UniversityManagementSystem.Application.DTOs.Attendance;
using UniversityManagementSystem.Application.DTOs.Chat;
using UniversityManagementSystem.Application.DTOs.Course;
using UniversityManagementSystem.Application.DTOs.Enrollment;
using UniversityManagementSystem.Application.DTOs.Results;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;
using UniversityManagementSystem.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUserManagementService _userService;
        private readonly IAttendanceService _attendanceService;
        private readonly IExamService _examService;
        private readonly IAssignmentService _assignmentService;
        private readonly IGradeService _gradeService;
        private readonly IChatService _chatService;
        private readonly IDashboardService _dashboardService;
        private readonly IResultService _resultService;
        private readonly ApplicationDbContext _context;

        public StudentController(
            IStudentService studentService,
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IUserManagementService userService,
            IAttendanceService attendanceService,
            IExamService examService,
            IAssignmentService assignmentService,
            IGradeService gradeService,
            IChatService chatService,
            IDashboardService dashboardService,
            IResultService resultService,
            ApplicationDbContext context)
        {
            _studentService = studentService;
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _userService = userService;
            _attendanceService = attendanceService;
            _examService = examService;
            _assignmentService = assignmentService;
            _gradeService = gradeService;
            _chatService = chatService;
            _dashboardService = dashboardService;
            _resultService = resultService;
            _context = context;
        }

        // === Dashboard ===
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");

            var studentName = string.Join(" ", new[] { student.FirstName, student.MiddleName, student.LastName }.Where(name => !string.IsNullOrWhiteSpace(name)));
            var model = await _dashboardService.GetStudentDashboardAsync(student.Id, studentName);
            return View(model);
        }

        // === Timetable ===

        /// <summary>
        /// Student Timetable: shows the admin-created timetable filtered to the
        /// student's enrolled courses. If no timetable exists, shows "not created yet".
        /// </summary>
        public async Task<IActionResult> StudentTimetable(int? academicYearId, int? semesterId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");

            // Check if any timetable entries exist at all
            var hasAnyTimetable = await _context.TimetableEntries.AnyAsync();
            ViewBag.HasAnyTimetable = hasAnyTimetable;

            // Load filter options
            var academicYears = await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync();
            ViewBag.AcademicYears = academicYears;

            // Auto-select current academic year
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

            // Auto-select first semester
            if (!semesterId.HasValue && semesters.Any())
            {
                semesterId = semesters.First().Id;
            }

            ViewBag.SelectedAcademicYearId = academicYearId;
            ViewBag.SelectedSemesterId = semesterId;

            // Get enrolled course titles for matching
            var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(student.Id);
            var courseTitles = enrollments
                .Where(e => string.Equals(e.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                .Select(e => e.CourseTitle.ToLower())
                .ToHashSet();
            var registeredCourseTitles = await _context.RegistrationItems
                .Where(i => i.Registration!.StudentId == student.Id
                    && i.Registration.Status == UniversityManagementSystem.Domain.Enums.RegistrationStatus.Approved
                    && i.Status == UniversityManagementSystem.Domain.Enums.RegistrationItemStatus.Registered
                    && (!semesterId.HasValue || i.Registration.SemesterId == semesterId.Value))
                .Select(i => i.SemesterOfferedCourse!.Course!.Title.ToLower())
                .ToListAsync();
            courseTitles.UnionWith(registeredCourseTitles);

            // Build query
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

            // Match by enrolled course titles
            var entries = await query
                .Where(t => courseTitles.Any(ct => t.Subject.ToLower().Contains(ct)))
                .OrderBy(t => t.Day)
                .ThenBy(t => t.Period != null ? t.Period.DisplayOrder : 0)
                .ToListAsync();

            // Get periods for the grid header
            var periods = await _context.Periods
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            ViewBag.Periods = periods;

            return View(entries);
        }

        // === Enrollment ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");

            try
            {
                var dto = new EnrollmentCreateDto { StudentId = student.Id, CourseId = courseId };
                await _enrollmentService.CreateEnrollmentAsync(dto);
                TempData["Success"] = "Enrollment request submitted.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Enrollments()
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");

            var registrations = await _context.RegistrationItems
                .Where(i => i.Registration!.StudentId == student.Id)
                .Include(i => i.Registration)
                    .ThenInclude(r => r!.Semester)
                .Include(i => i.SemesterOfferedCourse)
                    .ThenInclude(o => o!.Course)
                .ToListAsync();

            var rows = registrations
                .Where(i => i.SemesterOfferedCourse?.Course != null && i.Registration != null)
                .Select(i => new StudentEnrollmentRowViewModel
                {
                    CourseId = i.SemesterOfferedCourse!.CourseId,
                    CourseTitle = i.SemesterOfferedCourse.Course!.Title,
                    CourseCode = i.SemesterOfferedCourse.Course.Code,
                    SemesterName = i.Registration!.Semester?.Name ?? string.Empty,
                    Status = i.Registration.Status switch
                    {
                        UniversityManagementSystem.Domain.Enums.RegistrationStatus.Submitted => "Pending Review",
                        UniversityManagementSystem.Domain.Enums.RegistrationStatus.Approved
                            when i.Status == UniversityManagementSystem.Domain.Enums.RegistrationItemStatus.Registered => "Approved",
                        UniversityManagementSystem.Domain.Enums.RegistrationStatus.Approved
                            when i.Status == UniversityManagementSystem.Domain.Enums.RegistrationItemStatus.Waitlisted => "Waitlisted",
                        UniversityManagementSystem.Domain.Enums.RegistrationStatus.Rejected => "Rejected",
                        _ => i.Registration.Status.ToString()
                    },
                    EnrollmentDate = i.Registration.CreatedAt,
                    ApprovalDate = i.Registration.ReviewedAt
                })
                .ToList();

            var coursesRepresentedByActiveRegistrations = registrations
                .Where(i => i.Registration?.Status == UniversityManagementSystem.Domain.Enums.RegistrationStatus.Submitted
                    || i.Registration?.Status == UniversityManagementSystem.Domain.Enums.RegistrationStatus.Approved)
                .Select(i => i.SemesterOfferedCourse?.CourseId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToHashSet();

            var legacyEnrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(student.Id);
            rows.AddRange(legacyEnrollments
                .Where(e => !coursesRepresentedByActiveRegistrations.Contains(e.CourseId))
                .Select(e => new StudentEnrollmentRowViewModel
                {
                    CourseId = e.CourseId,
                    CourseTitle = e.CourseTitle,
                    CourseCode = e.CourseCode,
                    SemesterName = "—",
                    Status = e.Status,
                    EnrollmentDate = e.EnrollmentDate,
                    ApprovalDate = e.ApprovalDate
                }));

            return View(rows.OrderByDescending(row => row.EnrollmentDate).ToList());
        }

        /// <summary>
        /// My Courses: the student's approved enrollments rendered as course tiles.
        /// </summary>
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");

            var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(student.Id);
            var approved = enrollments.Where(e => e.Status == "Approved").ToList();
            var registeredCourseIds = await _context.RegistrationItems
                .Where(i => i.Registration!.StudentId == student.Id
                    && i.Registration.Status == UniversityManagementSystem.Domain.Enums.RegistrationStatus.Approved
                    && i.Status == UniversityManagementSystem.Domain.Enums.RegistrationItemStatus.Registered)
                .Select(i => i.SemesterOfferedCourse!.CourseId)
                .Distinct()
                .ToListAsync();
            var approvedCourseIds = approved.Select(e => e.CourseId).Concat(registeredCourseIds).Distinct().ToList();

            var courses = new List<CourseListDto>();
            foreach (var courseId in approvedCourseIds)
            {
                var view = await _courseService.GetCourseByIdAsync(courseId);
                if (view != null)
                {
                    courses.Add(new CourseListDto
                    {
                        Id = view.Id,
                        Title = view.Title,
                        Code = view.Code,
                        Credits = view.Credits,
                        InstructorName = view.InstructorName,
                        DepartmentName = view.DepartmentName
                    });
                    if (courses.Count >= 6) break;   // guard against runaway loops on huge enrollments
                }
            }

            ViewData["EnrollmentCount"] = approvedCourseIds.Count;
            return View(courses);
        }

        // === Attendance ===
        public async Task<IActionResult> MyAttendance(string? month = null)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            var attendances = await _attendanceService.GetAttendanceByStudentIdAsync(student.Id);

            // Filter by month if specified (format: YYYY-MM)
            if (!string.IsNullOrEmpty(month) && System.DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedMonth))
            {
                attendances = attendances.Where(a => a.Date.Year == parsedMonth.Year && a.Date.Month == parsedMonth.Month);
            }

            var summary = await _attendanceService.GetAttendanceSummaryAsync(student.Id);
            ViewBag.Summary = summary;
            ViewBag.Month = month ?? DateTime.Now.ToString("yyyy-MM");
            return View(attendances);
        }

        // === Exams ===
        public async Task<IActionResult> MyExams()
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            var exams = await _examService.GetExamsForStudentAsync(student.Id);
            return View(exams);
        }

        public async Task<IActionResult> TakeExam(int examId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            var exam = await _examService.GetExamWithQuestionsAsync(examId);
            // Check if student already submitted
            var result = await _examService.GetResultByStudentAndExamAsync(student.Id, examId);
            if (result != null)
                return RedirectToAction(nameof(ExamResult), new { examId });

            return View(exam);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(int examId, Dictionary<int, string> answers)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            await _examService.SubmitExamAsync(examId, student.Id, answers);
            TempData["Success"] = "Exam submitted successfully.";
            return RedirectToAction(nameof(ExamResult), new { examId });
        }

        public async Task<IActionResult> ExamResult(int examId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            var result = await _examService.GetResultByStudentAndExamAsync(student.Id, examId);
            if (result == null) return NotFound();
            return View(result);
        }

        // === Assignments ===
        public async Task<IActionResult> MyAssignments()
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            var assignments = await _assignmentService.GetAssignmentsForStudentAsync(student.Id);
            return View(assignments);
        }

        public async Task<IActionResult> SubmitAssignment(int assignmentId)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(assignmentId);
            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(SubmissionCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.GetCurrentUserAsync();
                    var student = await _studentService.GetStudentByUserIdAsync(user.Id);
                    if (student == null) return RedirectToAction("Index", "Home");
                    await _assignmentService.SubmitAssignmentAsync(dto, student.Id);
                    TempData["Success"] = "Assignment submitted.";
                    return RedirectToAction(nameof(MyAssignments));
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            var assignment = await _assignmentService.GetAssignmentByIdAsync(dto.AssignmentId);
            return View(assignment);
        }

        // === Assignment Answering ===
        public async Task<IActionResult> AnswerAssignment(int assignmentId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");

            // Check if already submitted
            var result = await _assignmentService.GetAssignmentResultByStudentAndAssignmentAsync(student.Id, assignmentId);
            if (result != null)
                return RedirectToAction(nameof(AssignmentResult), new { assignmentId });

            var assignment = await _assignmentService.GetAssignmentByIdAsync(assignmentId);
            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAssignmentAnswers(int assignmentId, Dictionary<int, string> answers)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            await _assignmentService.SubmitAssignmentAnswersAsync(assignmentId, student.Id, answers);
            TempData["Success"] = "Assignment submitted successfully.";
            return RedirectToAction(nameof(AssignmentResult), new { assignmentId });
        }

        public async Task<IActionResult> AssignmentResult(int assignmentId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            var result = await _assignmentService.GetAssignmentResultByStudentAndAssignmentAsync(student.Id, assignmentId);
            if (result == null) return NotFound();
            return View(result);
        }

        // === Grades & Results ===
        public async Task<IActionResult> MyGrades(int? semester = null)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return RedirectToAction("Index", "Home");
            var model = await _resultService.GetStudentTranscriptAsync(student.Id, semester);
            return View(model);
        }

        /// <summary>
        /// JSON breakdown of one published course result (used by the assessment modal).
        /// </summary>
        public async Task<JsonResult> CourseResultDetail(int courseId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null) return Json(null);

            var detail = await _resultService.GetStudentCourseResultDetailAsync(student.Id, courseId);
            if (detail == null) return Json(null);

            return Json(new
            {
                courseId = detail.CourseId,
                courseTitle = detail.CourseTitle,
                courseCode = detail.CourseCode,
                totalMark = detail.TotalMark,
                letterGrade = detail.LetterGrade,
                gradePoint = detail.GradePoint,
                assessments = detail.Assessments.Select(a => new
                {
                    name = a.Name,
                    weight = a.Weight,
                    score = a.Score
                }).ToList()
            });
        }

        // === Profile ===
        public async Task<IActionResult> Profile()
        {
            var user = await _userService.GetCurrentUserAsync();
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            return View(student);
        }
    }
}
