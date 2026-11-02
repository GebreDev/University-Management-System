using UniversityManagementSystem.Application.DTOs.Dashboard;
using UniversityManagementSystem.Application.Helpers;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    /// <summary>
    /// Aggregates real application data into rich, chart-ready models for the
    /// Admin, Instructor and Student dashboards.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IRegistrationRepository _registrationRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly ISubmissionRepository _submissionRepo;
        private readonly IExamRepository _examRepo;
        private readonly IEventRepository _eventRepo;
        private readonly ICourseScheduleRepository _scheduleRepo;
        private readonly INotificationRepository _notificationRepo;

        private static readonly string[] ChartColors = { "#2c6fbf", "#4a90e2", "#7ab6ea", "#1e5aa8", "#a8d5f5", "#5b8dd9", "#3e7cb1", "#c0e8ff" };

        // Ethiopia (Horn of Africa) uses East Africa Time: UTC+3 all year, no daylight saving.
        private static readonly TimeZoneInfo EthiopiaTimeZone = ResolveEthiopiaTimeZone();

        private static TimeZoneInfo ResolveEthiopiaTimeZone()
        {
            // .NET 6+ resolves IANA IDs on Windows too; fall back to the Windows ID,
            // then to a fixed UTC+3 offset so the greeting is always correct.
            try { return TimeZoneInfo.FindSystemTimeZoneById("Africa/Addis_Ababa"); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
            try { return TimeZoneInfo.FindSystemTimeZoneById("E. Africa Time"); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
            return TimeZoneInfo.CreateCustomTimeZone("EAT", TimeSpan.FromHours(3), "East Africa Time", "East Africa Time");
        }

        /// <summary>Current date/time in Ethiopia (EAT, UTC+3) regardless of server settings.</summary>
        private static DateTime EthiopiaNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EthiopiaTimeZone);

        public DashboardService(
            IStudentRepository studentRepo,
            IInstructorRepository instructorRepo,
            ICourseRepository courseRepo,
            IEnrollmentRepository enrollmentRepo,
            IRegistrationRepository registrationRepo,
            IGradeRepository gradeRepo,
            IAttendanceRepository attendanceRepo,
            IAssignmentRepository assignmentRepo,
            ISubmissionRepository submissionRepo,
            IExamRepository examRepo,
            IEventRepository eventRepo,
            ICourseScheduleRepository scheduleRepo,
            INotificationRepository notificationRepo)
        {
            _studentRepo = studentRepo;
            _instructorRepo = instructorRepo;
            _courseRepo = courseRepo;
            _enrollmentRepo = enrollmentRepo;
            _registrationRepo = registrationRepo;
            _gradeRepo = gradeRepo;
            _attendanceRepo = attendanceRepo;
            _assignmentRepo = assignmentRepo;
            _submissionRepo = submissionRepo;
            _examRepo = examRepo;
            _eventRepo = eventRepo;
            _scheduleRepo = scheduleRepo;
            _notificationRepo = notificationRepo;
        }

        // =====================================================================
        //  ADMIN
        // =====================================================================
        public async Task<AdminDashboardDto> GetAdminDashboardAsync(string adminName)
        {
            var dto = new AdminDashboardDto
            {
                AdminName = adminName,
                Greeting = GreetingFor(EthiopiaNow),
                Subtitle = "Here's what's happening across the university today.",
                Today = EthiopiaNow
            };

            var students = (await _studentRepo.GetAllStudentsWithDetailsAsync()).ToList();
            var instructors = (await _instructorRepo.GetAllInstructorsWithDetailsAsync()).ToList();
            var courses = (await _courseRepo.GetAllCoursesWithDetailsAsync()).ToList();
            var enrollments = (await _enrollmentRepo.GetAllEnrollmentsWithDetailsAsync()).ToList();
            var approvedRegistrationItems = (await _registrationRepo.GetApprovedItemsByCourseIdsAsync(courses.Select(c => c.Id))).ToList();
            var grades = (await _gradeRepo.GetAllAsync()).ToList();
            var events = (await _eventRepo.GetUpcomingAsync(5)).ToList();
            dto.PendingCourseRegistrations = await _registrationRepo.GetPendingRegistrationCountAsync();

            // ---- Metrics ----
            var now = DateTime.UtcNow;
            dto.Metrics = new List<DashboardMetric>
            {
                Metric("Total Students", students.Count.ToString("N0"), "from last month",
                    Trend(students.Count(s => s.CreatedAt >= now.AddMonths(-1)), students.Count(s => s.CreatedAt < now.AddMonths(-1) && s.CreatedAt >= now.AddMonths(-2))),
                    "fas fa-user-graduate", "stat-grad-a"),
                Metric("Total Instructors", instructors.Count.ToString("N0"), "from last month",
                    Trend(instructors.Count(t => t.CreatedAt >= now.AddMonths(-1)), instructors.Count(t => t.CreatedAt < now.AddMonths(-1) && t.CreatedAt >= now.AddMonths(-2))),
                    "fas fa-chalkboard-user", "stat-grad-c"),
                Metric("Total Programs", courses.Count.ToString("N0"), "courses offered",
                    Trend(courses.Count(c => c.CreatedAt >= now.AddMonths(-1)), courses.Count(c => c.CreatedAt < now.AddMonths(-1) && c.CreatedAt >= now.AddMonths(-2))),
                    "fas fa-book-open", "stat-grad-d"),
                Metric("Total Revenue", "N/A", "coming soon",
                    "—",
                    "fas fa-coins", "stat-grad-b")
            };

            // ---- Enrollment Overview (grouped bars) ----
            var months = LastMonths(7);
            var legacyApprovedCourses = enrollments
                .Where(e => e.Status == EnrollmentStatus.Approved)
                .Select(e => (e.StudentId, e.CourseId, EnrollmentDate: e.EnrollmentDate.Date))
                .ToHashSet();
            var registrationOnlyItems = approvedRegistrationItems
                .Where(i => i.Registration != null
                    && i.SemesterOfferedCourse != null
                    && !legacyApprovedCourses.Contains((
                        i.Registration.StudentId,
                        i.SemesterOfferedCourse.CourseId,
                        i.Registration.CreatedAt.Date)))
                .ToList();
            dto.Months = months.Select(m => m.ShortName).ToList();
            dto.ThisSemesterEnrollments = BucketCount(enrollments, e => e.EnrollmentDate, months)
                .Select((count, index) => count + registrationOnlyItems.Count(i =>
                    i.Registration!.CreatedAt.Year == months[index].Start.Year
                    && i.Registration.CreatedAt.Month == months[index].Start.Month))
                .ToList();
            dto.LastSemesterEnrollments = BucketCount(enrollments, e => e.EnrollmentDate.AddYears(-1), months)
                .Select((count, index) => count + registrationOnlyItems.Count(i =>
                    i.Registration!.CreatedAt.AddYears(-1).Year == months[index].Start.Year
                    && i.Registration.CreatedAt.AddYears(-1).Month == months[index].Start.Month))
                .ToList();

            // ---- Student Distribution (doughnut) ----
            var byDept = students.GroupBy(s => s.Department?.Name ?? "Unassigned")
                                 .OrderByDescending(g => g.Count())
                                 .ToList();
            var total = Math.Max(1, students.Count);
            dto.StudentDistribution = byDept.Select((g, i) => new ChartSlice
            {
                Label = g.Key,
                Value = g.Count(),
                Color = ChartColors[i % ChartColors.Length]
            }).ToList();

            // ---- Academic Performance (line) ----
            dto.PerfMonths = months.Select(m => m.ShortName).ToList();
            dto.GpaSeries = new List<double>();
            dto.PassRateSeries = new List<double>();
            dto.RetentionSeries = new List<double>();
            foreach (var m in months)
            {
                var monthGrades = grades.Where(g => g.CreatedAt.Year == m.Start.Year && g.CreatedAt.Month == m.Start.Month && g.MaxScore > 0).ToList();
                var pct = monthGrades.Select(g => (double)g.Score / (double)g.MaxScore * 100.0).ToList();
                dto.GpaSeries.Add(pct.Any() ? Math.Round(pct.Average(), 1) : 0);
                dto.PassRateSeries.Add(pct.Any() ? Math.Round(pct.Count(p => p >= 50) * 100.0 / pct.Count, 1) : 0);
                // Retention: share of students who enrolled on or before this month and are still active
                var activeByMonth = students.Count(s => s.EnrollmentDate.Date <= new DateTime(m.Start.Year, m.Start.Month, 1).AddMonths(1));
                var enrolledByMonth = Math.Max(1, students.Count(s => s.EnrollmentDate.Year < m.Start.Year || (s.EnrollmentDate.Year == m.Start.Year && s.EnrollmentDate.Month <= m.Start.Month)));
                dto.RetentionSeries.Add(Math.Round(activeByMonth * 100.0 / enrolledByMonth, 1));
            }

            // ---- Recent Activities (derived from real data) ----
            dto.RecentActivities = BuildRecentActivities(students, courses, enrollments);

            // ---- Upcoming Events ----
            dto.UpcomingEvents = events.Select(e => new EventItem
            {
                Day = e.StartDateTime.Day.ToString(),
                Month = e.StartDateTime.ToString("MMM"),
                Title = e.Title,
                Meta = $"{e.StartDateTime:hh:mm tt} - {e.Location}",
                ColorClass = "event-" + (string.IsNullOrWhiteSpace(e.Color) ? "primary" : e.Color)
            }).ToList();

            return dto;
        }

        // =====================================================================
        //  INSTRUCTOR
        // =====================================================================
        public async Task<InstructorDashboardDto> GetInstructorDashboardAsync(int instructorId, string instructorName)
        {
            var dto = new InstructorDashboardDto
            {
                InstructorName = instructorName,
                Greeting = GreetingFor(EthiopiaNow),
                Subtitle = "Here's your teaching overview for today.",
                Today = EthiopiaNow
            };

            var courses = (await _courseRepo.GetCoursesByInstructorIdAsync(instructorId)).ToList();
            var courseIds = courses.Select(c => c.Id).ToList();
            var enrollments = (await _enrollmentRepo.GetEnrollmentsByCourseIdsAsync(courseIds)).ToList();
            var approved = enrollments.Where(e => e.Status == EnrollmentStatus.Approved).ToList();
            var approvedRegistrationItems = (await _registrationRepo.GetApprovedItemsByCourseIdsAsync(courseIds)).ToList();
            var approvedStudentCourses = approved
                .Select(e => (StudentId: e.StudentId, CourseId: e.CourseId, EnrollmentDate: e.EnrollmentDate))
                .Concat(approvedRegistrationItems.Select(i => (
                    StudentId: i.Registration!.StudentId,
                    CourseId: i.SemesterOfferedCourse!.CourseId,
                    EnrollmentDate: i.Registration.CreatedAt)))
                .GroupBy(item => (item.StudentId, item.CourseId))
                .Select(group => group.OrderByDescending(item => item.EnrollmentDate).First())
                .ToList();

            var grades = (await _gradeRepo.GetByCourseIdsAsync(courseIds)).ToList();
            var attendance = (await _attendanceRepo.GetByCourseIdsAsync(courseIds)).ToList();
            var assignments = (await _assignmentRepo.GetByInstructorIdAsync(instructorId)).ToList();
            var assignmentIds = assignments.Select(a => a.Id).ToList();
            var submissions = (await _submissionRepo.GetByAssignmentIdsAsync(assignmentIds)).ToList();
            var exams = (await _examRepo.GetByInstructorIdAsync(instructorId)).ToList();
            var examIds = exams.Select(e => e.Id).ToList();
            var examResults = (await _examRepo.GetResultsByExamIdsAsync(examIds)).ToList();

            // ---- Metrics (trends computed from real data) ----
            var now = DateTime.UtcNow;
            var ungraded = submissions.Count(s => !s.IsGraded);
            var newEnrollSem = approvedStudentCourses.Count(e => e.EnrollmentDate >= now.AddMonths(-6));
            var prevEnrollSem = approvedStudentCourses.Count(e => e.EnrollmentDate < now.AddMonths(-6) && e.EnrollmentDate >= now.AddMonths(-12));
            var attNow = attendance.Where(a => a.Date >= now.AddMonths(-1)).ToList();
            var attPrev = attendance.Where(a => a.Date < now.AddMonths(-1) && a.Date >= now.AddMonths(-2)).ToList();
            dto.Metrics = new List<DashboardMetric>
            {
                Metric("My Courses", courses.Count.ToString(), "from last semester", Trend(courses.Count(c => c.CreatedAt >= now.AddMonths(-6)), courses.Count(c => c.CreatedAt < now.AddMonths(-6) && c.CreatedAt >= now.AddMonths(-12))), "fas fa-book-open", "stat-grad-a"),
                Metric("Total Students", approvedStudentCourses.Count.ToString("N0"), "this semester", Trend(newEnrollSem, prevEnrollSem), "fas fa-user-graduate", "stat-grad-c"),
                Metric("Pending Grading", ungraded.ToString(), "assignments", ungraded > 0 ? $"{ungraded} to review" : "All done", "fas fa-clipboard-check", "stat-grad-d"),
                Metric("Avg. Attendance", AttendanceRate(attendance) + "%", "this semester", Trend(AttendanceRate(attNow), AttendanceRate(attPrev)), "fas fa-calendar-check", "stat-grad-b")
            };

            // ---- Today's Schedule ----
            var today = EthiopiaNow; // match classes against Ethiopia's calendar day
            var schedules = await _scheduleRepo.GetByCourseIdsAsync(courseIds);
            var todays = schedules.Where(s => s.DayOfWeek == today.DayOfWeek).OrderBy(s => s.StartTime).ToList();
            dto.TodaySchedule = todays.Select(s => new ScheduleItem
            {
                Time = FormatTime(s.StartTime),
                Title = s.Course.Title,
                Code = s.Course.Code,
                Location = s.Room,
                Meta = $"{approvedStudentCourses.Count(e => e.CourseId == s.CourseId)} Students"
            }).ToList();

            // ---- Course Performance Overview (radar) ----
            var radarValues = new List<double> { 0, 0, 0, 0, 0 };
            if (courses.Any())
            {
                var gradePct = grades.Where(g => g.MaxScore > 0).Select(g => (double)g.Score / (double)g.MaxScore * 100).DefaultIfEmpty(0).Average();
                var attPct = AttendanceRate(attendance);
                var submittedPct = submissions.Any() ? submissions.Count * 100.0 / Math.Max(1, approvedStudentCourses.Count * Math.Max(1, assignments.Count)) : 0;
                var examPct = 0.0;
                if (examResults.Any() && exams.Any())
                    examPct = examResults.Average(r => r.TotalScore) * 100.0 / Math.Max(1, exams.Average(e => e.TotalMarks));
                var participation = (attPct + Math.Min(100, submittedPct)) / 2;
                radarValues = new List<double>
                {
                    Clamp(gradePct), Clamp(attPct), Clamp(submittedPct), Clamp(participation), Clamp(examPct)
                };
            }
            dto.RadarLabels = new List<string> { "Average Grade", "Attendance", "Assignments", "Participation", "Exams" };
            dto.RadarSeries = radarValues;

            // ---- My Courses grid ----
            dto.Courses = courses.Select(c =>
            {
                var studentCount = approvedStudentCourses.Count(e => e.CourseId == c.Id);
                var cGrades = grades.Where(g => g.CourseId == c.Id && g.MaxScore > 0).Select(g => (double)g.Score / (double)g.MaxScore * 100).ToList();
                var cAtt = attendance.Where(a => a.CourseId == c.Id).ToList();
                var avgGrade = cGrades.Any() ? LetterGrade(cGrades.Average()) : "—";
                var next = NextClass(schedules.Where(s => s.CourseId == c.Id).ToList(), today);
                return new CourseCardItem
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    Code = c.Code,
                    Students = studentCount,
                    Attendance = cAtt.Any() ? AttendanceRate(cAtt) + "%" : "—",
                    AvgGrade = avgGrade,
                    NextClass = next
                };
            }).Take(3).ToList();

            // ---- Pending Tasks ----
            var pendingResults = examResults.Count(r => string.IsNullOrWhiteSpace(r.Grade) && !r.IsPass);
            dto.PendingTasks = new List<TaskItem>
            {
                new TaskItem { Label = "Grade assignments", Count = $"{ungraded} Pending", Meta = "Submissions awaiting review", Icon = "fas fa-pen", Color = "primary" },
                new TaskItem { Label = "Review quiz submissions", Count = $"{pendingResults} Pending", Meta = "Manual essay grading", Icon = "fas fa-clipboard-list", Color = "info" },
                new TaskItem { Label = "Take attendance", Count = $"{todays.Count} Classes", Meta = "Today's schedule", Icon = "fas fa-calendar-check", Color = "success" },
                new TaskItem { Label = "Upcoming deadlines", Count = $"{assignments.Count(a => a.Deadline >= today)} Active", Meta = "Assignments in progress", Icon = "fas fa-hourglass-half", Color = "warning" }
            };

            // ---- Recent Student Activity ----
            dto.RecentActivity = BuildInstructorActivity(submissions, attendance, assignments, approved);

            return dto;
        }

        // =====================================================================
        //  STUDENT
        // =====================================================================
        public async Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId, string studentName)
        {
            var dto = new StudentDashboardDto
            {
                StudentName = studentName,
                Greeting = GreetingFor(EthiopiaNow),
                Subtitle = "Here's your academic overview.",
                Today = EthiopiaNow
            };

            var student = await _studentRepo.GetStudentWithDetailsAsync(studentId);
            if (student == null) return dto;

            var enrollments = (await _enrollmentRepo.GetEnrollmentsByStudentIdAsync(studentId)).ToList();
            var approved = enrollments.Where(e => e.Status == EnrollmentStatus.Approved).ToList();
            var approvedRegistrationItems = (await _registrationRepo.GetApprovedItemsByStudentAsync(studentId)).ToList();
            var courseIds = approved.Select(e => e.CourseId)
                .Concat(approvedRegistrationItems.Select(i => i.SemesterOfferedCourse!.CourseId))
                .Distinct()
                .ToList();

            var grades = (await _gradeRepo.GetByStudentIdAsync(studentId)).ToList();
            var attendance = (await _attendanceRepo.GetByStudentIdAsync(studentId)).ToList();
            var notifications = (await _notificationRepo.GetByUserIdAsync(student.ApplicationUserId)).ToList();

            var allCourses = (await _courseRepo.GetAllCoursesWithDetailsAsync()).ToList();
            var enrolledCourseIds = courseIds.ToHashSet();
            var assignments = (await _assignmentRepo.GetByCourseIdsAsync(enrolledCourseIds)).Where(a => a.IsPublished).ToList();

            // ---- Metrics ----
            var graded = grades.Where(g => g.MaxScore > 0).ToList();
            var gpaPct = graded.Any() ? graded.Average(g => (double)g.Score / (double)g.MaxScore * 100.0) : 0;
            var gpa = gpaPct / 25.0;
            var credits = courseIds.Select(courseId => allCourses.FirstOrDefault(c => c.Id == courseId)?.Credits ?? 0).Sum();
            var totalTarget = 120;
            var attRate = AttendanceRate(attendance);

            dto.Metrics = new List<DashboardMetric>
            {
                Metric("GPA", gpa.ToString("F2"), "Out of 4.00", credits >= 120 ? "Graduating" : $"{Math.Round(credits * 100.0 / totalTarget)}% complete", "fas fa-star", "stat-grad-a"),
                Metric("Credits Completed", $"{credits} / {totalTarget}", "this semester", $"{Math.Round(credits * 100.0 / totalTarget)}% Complete", "fas fa-layer-group", "stat-grad-c"),
                Metric("Attendance", attRate + "%", "this semester", attRate >= 90 ? "Excellent" : "Keep it up", "fas fa-calendar-check", "stat-grad-d"),
                Metric("Notifications", notifications.Count.ToString(), "total", $"{notifications.Count(n => !n.IsRead)} unread", "fas fa-bell", "stat-grad-b")
            };

            // ---- My Timetable - Today ----
            var schedules = await _scheduleRepo.GetByCourseIdsAsync(courseIds);
            var today = EthiopiaNow; // match classes against Ethiopia's calendar day
            var todays = schedules.Where(s => s.DayOfWeek == today.DayOfWeek).OrderBy(s => s.StartTime).ToList();
            dto.Timetable = todays.Select(s =>
            {
                var course = allCourses.FirstOrDefault(c => c.Id == s.CourseId);
                var cAtt = attendance.Where(a => a.CourseId == s.CourseId).ToList();
                return new ScheduleItem
                {
                    Time = FormatTime(s.StartTime),
                    Title = s.Course.Title,
                    Code = s.Course.Code,
                    Location = s.Room,
                    Meta = course?.Instructor != null ? $"{course.Instructor.FirstName} {course.Instructor.LastName}" : "",
                    Progress = cAtt.Any() ? AttendanceRate(cAtt).ToString() : "0"
                };
            }).ToList();

            // ---- Upcoming Assignments ----
            var upcoming = assignments.Where(a => a.Deadline >= today).OrderBy(a => a.Deadline).Take(4).ToList();
            var upcomingItems = new List<AssignmentItem>();
            foreach (var a in upcoming)
            {
                var result = await _submissionRepo.GetByStudentAndAssignmentAsync(studentId, a.Id);
                var progress = result != null ? 100 : ProgressByDates(a.CreatedAt, a.Deadline, today);
                upcomingItems.Add(new AssignmentItem
                {
                    AssignmentId = a.Id,
                    Title = a.Title,
                    Due = "Due: " + a.Deadline.ToString("MMM dd, yyyy"),
                    Progress = progress.ToString()
                });
            }
            dto.UpcomingAssignments = upcomingItems;

            // ---- My Courses grid ----
            dto.Courses = courseIds.Select(courseId =>
            {
                var course = allCourses.FirstOrDefault(c => c.Id == courseId);
                if (course == null) return null;
                var cGrades = grades.Where(g => g.CourseId == courseId && g.MaxScore > 0).Select(g => (double)g.Score / (double)g.MaxScore * 100).ToList();
                var cAtt = attendance.Where(a => a.CourseId == courseId).ToList();
                return new CourseCardItem
                {
                    CourseId = course.Id,
                    Title = course.Title,
                    Code = course.Code,
                    Instructor = course.Instructor != null ? $"{course.Instructor.FirstName} {course.Instructor.LastName}" : "",
                    AvgGrade = cGrades.Any() ? LetterGrade(cGrades.Average()) : "—",
                    Attendance = cAtt.Any() ? AttendanceRate(cAtt) + "%" : "—"
                };
            }).OfType<CourseCardItem>().Take(4).ToList();

            // ---- Announcements ----
            dto.Announcements = notifications.Where(n => n.Type == "Announcement")
                .OrderByDescending(n => n.CreatedAt).Take(4)
                .Select(n => new AnnouncementItem
                {
                    Title = n.Title,
                    Date = n.CreatedAt.ToString("MMM dd, yyyy"),
                    Icon = "fas fa-bullhorn",
                    Color = "primary"
                }).ToList();

            // ---- Recent Grades ----
            dto.RecentGrades = grades.OrderByDescending(g => g.CreatedAt).Take(4)
                .Select(g => new GradeItem
                {
                    Title = g.Course.Title,
                    Grade = string.IsNullOrWhiteSpace(g.LetterGrade) ? (g.MaxScore > 0 ? LetterGrade((double)g.Score / (double)g.MaxScore * 100) : "—") : g.LetterGrade,
                    Date = g.CreatedAt.ToString("MMM dd")
                }).ToList();

            return dto;
        }

        // =====================================================================
        //  HELPERS
        // =====================================================================

        private static string GreetingFor(DateTime now)
        {
            // Thresholds tuned for Ethiopia's roughly 6am–6:30pm daylight window.
            var h = now.Hour;
            if (h < 6) return "Good evening";   // after midnight – still evening
            if (h < 12) return "Good morning";
            if (h < 18) return "Good afternoon";
            return "Good evening";
        }

        private static DashboardMetric Metric(string label, string value, string subLabel, string trend, string icon, string gradient)
            => new DashboardMetric
            {
                Label = label,
                Value = value,
                SubLabel = subLabel,
                Trend = trend,
                TrendPositive = !trend.StartsWith("-") || trend == "—",
                Icon = icon,
                GradientClass = gradient
            };

        private static string Trend(int current, int previous)
        {
            if (previous <= 0) return current > 0 ? "+100%" : "—";
            var diff = (double)(current - previous) / previous * 100.0;
            return (diff >= 0 ? "+" : "") + diff.ToString("F1") + "%";
        }

        private class MonthBucket
        {
            public string ShortName { get; set; } = "";
            public DateTime Start { get; set; }
        }

        private static List<MonthBucket> LastMonths(int count)
        {
            var list = new List<MonthBucket>();
            var now = DateTime.UtcNow;
            for (int i = count - 1; i >= 0; i--)
            {
                var start = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                list.Add(new MonthBucket { ShortName = start.ToString("MMM"), Start = start });
            }
            return list;
        }

        private static List<int> BucketCount<T>(IEnumerable<T> items, Func<T, DateTime> dateSelector, List<MonthBucket> buckets)
        {
            var result = new List<int>();
            foreach (var b in buckets)
                result.Add(items.Count(x => dateSelector(x).Year == b.Start.Year && dateSelector(x).Month == b.Start.Month));
            return result;
        }

        private static int AttendanceRate(IEnumerable<Attendance> attendance)
        {
            var list = attendance.ToList();
            if (!list.Any()) return 0;
            var present = list.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Excused);
            return (int)Math.Round(present * 100.0 / list.Count);
        }

        private static string LetterGrade(double pct)
        {
            if (pct >= 90) return "A+";
            if (pct >= 85) return "A";
            if (pct >= 80) return "A-";
            if (pct >= 75) return "B+";
            if (pct >= 70) return "B";
            if (pct >= 65) return "B-";
            if (pct >= 60) return "C+";
            if (pct >= 55) return "C";
            if (pct >= 50) return "C-";
            if (pct >= 40) return "D";
            return "F";
        }

        private static double Clamp(double v) => Math.Max(0, Math.Min(100, v));

        private static string FormatTime(TimeSpan t)
        {
            var dt = DateTime.Today.Add(t);
            return dt.ToString("hh:mm tt");
        }

        private static string NextClass(List<CourseSchedule> schedules, DateTime today)
        {
            if (!schedules.Any()) return "No schedule";
            var ordered = schedules.OrderBy(s => ((int)s.DayOfWeek - (int)today.DayOfWeek + 7) % 7).ThenBy(s => s.StartTime);
            var next = ordered.First();
            var daysAhead = ((int)next.DayOfWeek - (int)today.DayOfWeek + 7) % 7;
            var prefix = daysAhead == 0 ? "Today" : daysAhead == 1 ? "Tomorrow" : next.DayOfWeek.ToString();
            return $"{prefix}, {FormatTime(next.StartTime)}";
        }

        private static int ProgressByDates(DateTime start, DateTime deadline, DateTime now)
        {
            var total = (deadline - start).TotalDays;
            if (total <= 0) return 0;
            var elapsed = (now - start).TotalDays;
            return (int)Math.Max(0, Math.Min(100, elapsed / total * 100));
        }

        private static string TimeAgo(DateTime dt)
        {
            var diff = DateTime.UtcNow - dt;
            if (diff.TotalMinutes < 1) return "just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hour{(diff.TotalHours >= 2 ? "s" : "")} ago";
            if (diff.TotalDays < 30) return $"{(int)diff.TotalDays} day{(diff.TotalDays >= 2 ? "s" : "")} ago";
            return dt.ToString("MMM dd");
        }

        private static List<ActivityItem> BuildRecentActivities(
            List<Student> students, List<Course> courses, List<Enrollment> enrollments)
        {
            var items = new List<ActivityItem>();

            foreach (var s in students.OrderByDescending(s => s.CreatedAt).Take(3))
                items.Add(new ActivityItem
                {
                    Icon = "fas fa-user-plus",
                    Color = "success",
                    Title = "New student registered",
                    Detail = $"{s.FirstName} {s.LastName} - {s.Department?.Name ?? "General"}",
                    TimeAgo = TimeAgo(s.CreatedAt),
                    Timestamp = s.CreatedAt
                });

            foreach (var c in courses.OrderByDescending(c => c.CreatedAt).Take(2))
                items.Add(new ActivityItem
                {
                    Icon = "fas fa-book",
                    Color = "primary",
                    Title = "Course created",
                    Detail = c.Title,
                    TimeAgo = TimeAgo(c.CreatedAt),
                    Timestamp = c.CreatedAt
                });

            foreach (var e in enrollments.Where(e => e.Status == EnrollmentStatus.Approved).OrderByDescending(e => e.ApprovalDate ?? e.EnrollmentDate).Take(2))
                items.Add(new ActivityItem
                {
                    Icon = "fas fa-check-circle",
                    Color = "warning",
                    Title = "Enrollment approved",
                    Detail = $"{e.Student?.FirstName} {e.Student?.LastName} - {e.Course?.Title}",
                    TimeAgo = TimeAgo(e.ApprovalDate ?? e.EnrollmentDate),
                    Timestamp = e.ApprovalDate ?? e.EnrollmentDate
                });

            return items.OrderByDescending(i => i.Timestamp).Take(5).ToList();
        }

        private static List<ActivityItem> BuildInstructorActivity(
            List<Submission> submissions, List<Attendance> attendance, List<Assignment> assignments, List<Enrollment> approved)
        {
            var items = new List<ActivityItem>();

            foreach (var s in submissions.OrderByDescending(s => s.SubmittedAt).Take(3))
                items.Add(new ActivityItem
                {
                    Icon = "fas fa-file-upload",
                    Color = "primary",
                    Title = "Assignment submitted",
                    Detail = $"{s.Student?.FirstName} {s.Student?.LastName} - {s.Assignment?.Title}",
                    TimeAgo = TimeAgo(s.SubmittedAt),
                    Timestamp = s.SubmittedAt
                });

            foreach (var a in attendance.Where(a => a.Status == AttendanceStatus.Absent).OrderByDescending(a => a.Date).Take(3))
                items.Add(new ActivityItem
                {
                    Icon = "fas fa-user-times",
                    Color = "danger",
                    Title = "Student absent",
                    Detail = $"{a.Student?.FirstName} {a.Student?.LastName} - {a.Course?.Title}",
                    TimeAgo = TimeAgo(a.Date),
                    Timestamp = a.Date
                });

            foreach (var e in approved.OrderByDescending(e => e.EnrollmentDate).Take(2))
                items.Add(new ActivityItem
                {
                    Icon = "fas fa-user-graduate",
                    Color = "success",
                    Title = "New enrollment",
                    Detail = $"{e.Student?.FirstName} {e.Student?.LastName} - {e.Course?.Title}",
                    TimeAgo = TimeAgo(e.EnrollmentDate),
                    Timestamp = e.EnrollmentDate
                });

            return items.OrderByDescending(i => i.Timestamp).Take(4).ToList();
        }
    }
}
