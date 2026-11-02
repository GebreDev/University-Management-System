using UniversityManagementSystem.Application.DTOs.Attendance;
using UniversityManagementSystem.Application.DTOs.Chat;
using UniversityManagementSystem.Application.DTOs.Promotion;
using UniversityManagementSystem.Application.DTOs.Course;
using UniversityManagementSystem.Application.DTOs.Department;
using UniversityManagementSystem.Application.DTOs.Enrollment;
using UniversityManagementSystem.Application.DTOs.Reports;
using UniversityManagementSystem.Application.DTOs.Student;
using UniversityManagementSystem.Application.DTOs.Instructor;
using UniversityManagementSystem.Application.DTOs.UserManagement;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Helpers;
using UniversityManagementSystem.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Existing services
        private readonly IDepartmentService _departmentService;
        private readonly IInstructorService _instructorService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        // New services
        private readonly IUserManagementService _userManagementService;
        private readonly IAttendanceService _attendanceService;
        private readonly IReportService _reportService;
        private readonly IAuditLogService _auditLogService;
        private readonly IChatService _chatService;
        private readonly IDashboardService _dashboardService;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            IDepartmentService departmentService,
            IInstructorService instructorService,
            IStudentService studentService,
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IUserManagementService userManagementService,
            IAttendanceService attendanceService,
            IReportService reportService,
            IAuditLogService auditLogService,
            IChatService chatService,
            IDashboardService dashboardService,
            IFileService fileService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _departmentService = departmentService;
            _instructorService = instructorService;
            _studentService = studentService;
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _userManagementService = userManagementService;
            _attendanceService = attendanceService;
            _reportService = reportService;
            _auditLogService = auditLogService;
            _chatService = chatService;
            _dashboardService = dashboardService;
            _fileService = fileService;
            _context = context;
            _userManager = userManager;
        }

        // ===== DASHBOARD =====
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManagementService.GetCurrentUserAsync();
            var adminName = string.Join(" ", new[] { user.FirstName, user.MiddleName, user.LastName }.Where(name => !string.IsNullOrWhiteSpace(name)));
            var model = await _dashboardService.GetAdminDashboardAsync(adminName);
            return View(model);
        }

        // ===== EXISTING CRUD – DEPARTMENTS =====
        #region Departments
        public async Task<IActionResult> Departments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View("Departments/Index", departments);
        }

        public IActionResult CreateDepartment()
        {
            return View("Departments/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(DepartmentCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.CreateDepartmentAsync(dto);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Create", "Department", null, $"Created department '{dto.Name}'");
                return RedirectToAction(nameof(Departments));
            }
            return View("Departments/Create", dto);
        }

        public async Task<IActionResult> EditDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();
            var updateDto = new DepartmentUpdateDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                Code = department.Code
            };
            return View("Departments/Edit", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(DepartmentUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.UpdateDepartmentAsync(dto);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Update", "Department", dto.Id, $"Updated department '{dto.Name}'");
                return RedirectToAction(nameof(Departments));
            }
            return View("Departments/Edit", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                await _departmentService.DeleteDepartmentAsync(id);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Delete", "Department", id, $"Deleted department ID {id}");
                TempData["Success"] = "Department deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete department: {ex.Message}";
            }
            return RedirectToAction(nameof(Departments));
        }

        public async Task<IActionResult> DetailsDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();
            return View("Departments/Details", department);
        }
        #endregion

        // ===== EXISTING CRUD – INSTRUCTORS =====
        #region Instructors
        public async Task<IActionResult> Instructors()
        {
            var instructors = await _instructorService.GetAllInstructorsAsync();
            return View("Instructors/Index", instructors);
        }

        public async Task<IActionResult> CreateInstructor()
        {
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Regions = new SelectList(await _context.Regions.OrderBy(r => r.Name).ToListAsync(), "RegionId", "Name");
            ViewBag.AcademicGrades = new SelectList(await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            ViewBag.Courses = new SelectList(await _context.Courses.OrderBy(c => c.Title).ToListAsync(), "Id", "Title");
            var viewModel = new InstructorCreateViewModel
            {
                CourseAssignments = new List<InstructorCourseAssignment> { new InstructorCourseAssignment() }
            };
            return View("Instructors/Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInstructor(InstructorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle profile picture upload
                    string? profilePictureUrl = null;
                    if (model.ProfilePicture != null)
                    {
                        profilePictureUrl = await _fileService.SaveFileAsync(model.ProfilePicture, "profiles");
                    }

                    var dto = new InstructorCreateDto
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Qualification = model.Qualification,
                        HireDate = model.HireDate,
                        DepartmentId = model.DepartmentId,
                        Password = model.Password,
                        Username = model.Username // Instructor login username
                    };

                    var result = await _instructorService.CreateInstructorAsync(dto);

                    // Update instructor with additional fields
                    var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == result.Id);
                    if (instructor != null)
                    {
                        instructor.MiddleName = model.MiddleName;
                        instructor.Gender = model.Gender;
                        instructor.DateOfBirth = model.DateOfBirth;
                        instructor.RegionId = model.RegionId;
                        instructor.ZoneId = model.ZoneId;
                        instructor.WoredaId = model.WoredaId;
                        instructor.KebeleId = model.KebeleId;
                        instructor.ProfilePictureUrl = profilePictureUrl;
                        var instructorUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == instructor.ApplicationUserId);
                        if (instructorUser != null)
                        {
                            instructorUser.MiddleName = model.MiddleName;
                        }
                        await _context.SaveChangesAsync();
                    }

                    var user = await _userManagementService.GetCurrentUserAsync();
                    await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Create", "Instructor", null, $"Created instructor '{dto.FirstName} {dto.LastName}'");
                    TempData["Success"] = "Instructor registered successfully.";
                    return RedirectToAction(nameof(Instructors));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Regions = new SelectList(await _context.Regions.OrderBy(r => r.Name).ToListAsync(), "RegionId", "Name");
            ViewBag.AcademicGrades = new SelectList(await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            ViewBag.Courses = new SelectList(await _context.Courses.OrderBy(c => c.Title).ToListAsync(), "Id", "Title");
            return View("Instructors/Create", model);
        }

        public async Task<IActionResult> EditInstructor(int id)
        {
            var instructor = await _instructorService.GetInstructorByIdAsync(id);
            if (instructor == null) return NotFound();
            var updateDto = new InstructorUpdateDto
            {
                Id = instructor.Id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                Email = instructor.Email,
                Phone = instructor.Phone,
                Qualification = instructor.Qualification,
                DepartmentId = instructor.DepartmentId
            };
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View("Instructors/Edit", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInstructor(InstructorUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _instructorService.UpdateInstructorAsync(dto);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Update", "Instructor", dto.Id, $"Updated instructor ID {dto.Id}");
                return RedirectToAction(nameof(Instructors));
            }
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View("Instructors/Edit", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            try
            {
                await _instructorService.DeleteInstructorAsync(id);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Delete", "Instructor", id, $"Deleted instructor ID {id}");
                TempData["Success"] = "Instructor deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete instructor: {ex.Message}";
            }
            return RedirectToAction(nameof(Instructors));
        }

        public async Task<IActionResult> DetailsInstructor(int id)
        {
            var instructor = await _instructorService.GetInstructorByIdAsync(id);
            if (instructor == null) return NotFound();
            return View("Instructors/Details", instructor);
        }
        #endregion

        // ===== EXISTING CRUD – STUDENTS =====
        #region Students
        public async Task<IActionResult> Students()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View("Students/Index", students);
        }

        public async Task<IActionResult> CreateStudent()
        {
            ViewBag.Regions = new SelectList(await _context.Regions.OrderBy(r => r.Name).ToListAsync(), "RegionId", "Name");
            ViewBag.AcademicGrades = new SelectList(await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View("Students/Create", new StudentCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file uploads
                    string? profilePictureUrl = null;
                    string? studentDocumentPath = null;

                    if (model.ProfilePhoto != null)
                    {
                        profilePictureUrl = await _fileService.SaveFileAsync(model.ProfilePhoto, "profiles");
                    }

                    if (model.StudentDocument != null)
                    {
                        studentDocumentPath = await _fileService.SaveFileAsync(model.StudentDocument, "submissions");
                    }

                    var dto = new StudentCreateDto
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        StudentId = model.StudentId,
                        EnrollmentDate = DateTime.Now,
                        DepartmentId = await _context.Departments.Select(d => d.Id).FirstOrDefaultAsync(),
                        Password = model.Password,
                        Username = model.StudentId // Student login username = UGR (StudentId)
                    };

                    var result = await _studentService.CreateStudentAsync(dto);

                    // Update the student with additional fields
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == result.Id);
                    if (student != null)
                    {
                        student.MiddleName = model.MiddleName;
                        student.Gender = model.Gender;
                        student.Nationality = model.Nationality;
                        student.DateOfBirth = model.DateOfBirth;
                        student.RegionId = model.RegionId;
                        student.ZoneId = model.ZoneId;
                        student.WoredaId = model.WoredaId;
                        student.KebeleId = model.KebeleId;
                        student.AcademicGradeId = model.AcademicGradeId;
                        student.AcademicSectionId = model.AcademicSectionId;
                        student.ProfilePictureUrl = profilePictureUrl;
                        student.StudentDocumentPath = studentDocumentPath;
                        var studentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == student.ApplicationUserId);
                        if (studentUser != null)
                        {
                            studentUser.MiddleName = model.MiddleName;
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Create parent account if requested
                    if (model.RegisterParent && !string.IsNullOrWhiteSpace(model.ParentFirstName) && !string.IsNullOrWhiteSpace(model.ParentLastName))
                    {
                        string? parentProfilePictureUrl = null;
                        if (model.ParentProfilePhoto != null)
                        {
                            parentProfilePictureUrl = await _fileService.SaveFileAsync(model.ParentProfilePhoto, "profiles");
                        }

                        // Create parent user account
                        var parentUser = new ApplicationUser
                        {
                            UserName = model.ParentUsername,
                            Email = model.ParentEmail,
                            FirstName = model.ParentFirstName,
                            LastName = model.ParentLastName,
                            CreatedAt = DateTime.UtcNow,
                            Address = ""
                        };
                        var parentResult = await _userManager.CreateAsync(parentUser, model.ParentPassword);
                        if (parentResult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(parentUser, "Student");

                            var parent = new Parent
                            {
                                FirstName = model.ParentFirstName,
                                MiddleName = model.ParentMiddleName,
                                LastName = model.ParentLastName,
                                Username = model.ParentUsername,
                                Email = model.ParentEmail,
                                Phone = model.ParentPhone,
                                Nationality = model.ParentNationality,
                                EconomicStatus = model.EconomicStatus,
                                ProfilePictureUrl = parentProfilePictureUrl,
                                StudentId = result.Id,
                                ApplicationUserId = parentUser.Id
                            };
                            _context.Parents.Add(parent);
                            await _context.SaveChangesAsync();
                        }
                    }

                    var adminUser = await _userManagementService.GetCurrentUserAsync();
                    await _auditLogService.LogAsync(adminUser.Id, $"{adminUser.FirstName} {adminUser.LastName}", "Admin", "Create", "Student", null, $"Created student '{dto.FirstName} {dto.LastName}'");
                    TempData["Success"] = "Student registered successfully.";
                    return RedirectToAction(nameof(Students));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.Regions = new SelectList(await _context.Regions.OrderBy(r => r.Name).ToListAsync(), "RegionId", "Name");
            ViewBag.AcademicGrades = new SelectList(await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View("Students/Create", model);
        }

        // AJAX endpoints for cascading dropdowns
        [HttpGet]
        public async Task<IActionResult> GetZones(int regionId)
        {
            var zones = await _context.Zones
                .Where(z => z.RegionId == regionId)
                .OrderBy(z => z.Name)
                .Select(z => new { id = z.ZoneId, name = z.Name })
                .ToListAsync();
            return Json(zones);
        }

        [HttpGet]
        public async Task<IActionResult> GetWoredas(int zoneId)
        {
            var woredas = await _context.Woredas
                .Where(w => w.ZoneId == zoneId)
                .OrderBy(w => w.Name)
                .Select(w => new { id = w.WoredaId, name = w.Name })
                .ToListAsync();
            return Json(woredas);
        }

        [HttpGet]
        public async Task<IActionResult> GetKebeles(int woredaId)
        {
            var kebeles = await _context.Kebeles
                .Where(k => k.WoredaId == woredaId)
                .OrderBy(k => k.Name)
                .Select(k => new { id = k.KebeleId, name = k.Name })
                .ToListAsync();
            return Json(kebeles);
        }

        [HttpGet]
        public async Task<IActionResult> GetSections(int gradeId)
        {
            var sections = await _context.AcademicSections
                .Where(s => s.GradeId == gradeId)
                .OrderBy(s => s.Name)
                .Select(s => new { id = s.Id, name = s.Name })
                .ToListAsync();
            return Json(sections);
        }

        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();
            var updateDto = new StudentUpdateDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Phone = student.Phone,
                DepartmentId = student.DepartmentId
            };
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View("Students/Edit", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(StudentUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _studentService.UpdateStudentAsync(dto);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Update", "Student", dto.Id, $"Updated student ID {dto.Id}");
                return RedirectToAction(nameof(Students));
            }
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View("Students/Edit", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Delete", "Student", id, $"Deleted student ID {id}");
                TempData["Success"] = "Student deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete student: {ex.Message}";
            }
            return RedirectToAction(nameof(Students));
        }

        public async Task<IActionResult> DetailsStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();
            return View("Students/Details", student);
        }
        #endregion

        // ===== EXISTING CRUD – COURSES =====
        #region Courses
        public async Task<IActionResult> Courses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View("Courses/Index", courses);
        }

        public async Task<IActionResult> CreateCourse()
        {
            await PopulateCourseCreateDropdownsAsync();
            return View("Courses/Create", new CourseCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CourseCreateViewModel model)
        {
            if (ModelState.IsValid && !await _context.Semesters.AnyAsync(s => s.Id == model.SemesterId))
                ModelState.AddModelError(nameof(model.SemesterId), "Select a valid semester.");

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var dto = new CourseCreateDto
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Code = model.Code,
                        Credits = model.Credits,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        DepartmentId = model.DepartmentId,
                        InstructorId = model.InstructorId
                    };

                    var result = await _courseService.CreateCourseAsync(dto);

                    _context.SemesterOfferedCourses.Add(new SemesterOfferedCourse
                    {
                        CourseId = result.Id,
                        SemesterId = model.SemesterId,
                        DepartmentId = model.DepartmentId,
                        Capacity = model.RegistrationCapacity,
                        IsOpen = true
                    });

                    // Save course-year associations
                    if (model.SelectedYearIds != null && model.SelectedYearIds.Any())
                    {
                        foreach (var yearId in model.SelectedYearIds)
                        {
                            var courseGrade = new CourseGrade
                            {
                                CourseId = result.Id,
                                GradeId = yearId
                            };
                            _context.CourseGrades.Add(courseGrade);
                        }
                    }

                    await _context.SaveChangesAsync();

                    var user = await _userManagementService.GetCurrentUserAsync();
                    await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Create", "Course", null, $"Created course '{dto.Title}'");
                    await transaction.CommitAsync();
                    TempData["Success"] = "Course created successfully.";
                    return RedirectToAction(nameof(Courses));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await PopulateCourseCreateDropdownsAsync();
            return View("Courses/Create", model);
        }

        private async Task PopulateCourseCreateDropdownsAsync()
        {
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Instructors = await _instructorService.GetAllInstructorsAsync();
            ViewBag.AcademicGrades = new SelectList(
                await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync(),
                "Id",
                "Name");
            ViewBag.Semesters = await _context.Semesters
                .AsNoTracking()
                .Include(s => s.AcademicYear)
                .OrderByDescending(s => s.StartDate)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name + " - " + s.AcademicYear!.Name
                })
                .ToListAsync();
        }

        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            var updateDto = new CourseUpdateDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Code = course.Code,
                Credits = course.Credits,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                DepartmentId = course.DepartmentId,
                InstructorId = course.InstructorId
            };
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Instructors = await _instructorService.GetAllInstructorsAsync();
            return View("Courses/Edit", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(CourseUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _courseService.UpdateCourseAsync(dto);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Update", "Course", dto.Id, $"Updated course ID {dto.Id}");
                return RedirectToAction(nameof(Courses));
            }
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Instructors = await _instructorService.GetAllInstructorsAsync();
            return View("Courses/Edit", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Delete", "Course", id, $"Deleted course ID {id}");
                TempData["Success"] = "Course deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete course: {ex.Message}";
            }
            return RedirectToAction(nameof(Courses));
        }

        public async Task<IActionResult> DetailsCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return View("Courses/Details", course);
        }
        #endregion

        // ===== EXISTING CRUD – ENROLLMENTS =====
        #region Enrollments
        public async Task<IActionResult> Enrollments()
        {
            var enrollments = await GetAdminEnrollmentRowsAsync();
            return View("Enrollments/Index", enrollments);
        }

        public async Task<IActionResult> PendingEnrollments()
        {
            var pending = await GetAdminEnrollmentRowsAsync(pendingOnly: true);
            return View("Enrollments/Pending", pending);
        }

        private async Task<List<AdminEnrollmentRowViewModel>> GetAdminEnrollmentRowsAsync(bool pendingOnly = false)
        {
            var legacyEnrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            var registrationItems = await _context.RegistrationItems
                .Include(i => i.Registration)
                    .ThenInclude(r => r!.Student)
                .Include(i => i.Registration)
                    .ThenInclude(r => r!.Semester)
                .Include(i => i.SemesterOfferedCourse)
                    .ThenInclude(o => o!.Course)
                .ToListAsync();

            var rows = legacyEnrollments
                .Where(e => !pendingOnly || e.Status == UniversityManagementSystem.Domain.Enums.EnrollmentStatus.Pending)
                .Select(e => new AdminEnrollmentRowViewModel
                {
                    Id = e.Id,
                    StudentFullName = string.Join(" ", new[]
                    {
                        e.Student.FirstName,
                        e.Student.MiddleName,
                        e.Student.LastName
                    }.Where(name => !string.IsNullOrWhiteSpace(name))),
                    StudentId = e.Student.StudentId,
                    CourseTitle = e.Course.Title,
                    CourseCode = e.Course.Code,
                    Status = e.Status.ToString(),
                    EnrollmentDate = e.EnrollmentDate,
                    ApprovalDate = e.ApprovalDate
                })
                .ToList();

            rows.AddRange(registrationItems
                .Where(i => i.Registration?.Student != null
                    && i.SemesterOfferedCourse?.Course != null
                    && (!pendingOnly || i.Registration.Status == UniversityManagementSystem.Domain.Enums.RegistrationStatus.Submitted)
                    && !legacyEnrollments.Any(e =>
                        e.StudentId == i.Registration.StudentId
                        && e.CourseId == i.SemesterOfferedCourse.CourseId
                        && e.EnrollmentDate == i.Registration.CreatedAt))
                .Select(i => new AdminEnrollmentRowViewModel
                {
                    Id = i.Id,
                    RegistrationId = i.Registration!.Id,
                    StudentFullName = string.Join(" ", new[]
                    {
                        i.Registration.Student!.FirstName,
                        i.Registration.Student.MiddleName,
                        i.Registration.Student.LastName
                    }.Where(name => !string.IsNullOrWhiteSpace(name))),
                    StudentId = i.Registration.Student!.StudentId,
                    CourseTitle = i.SemesterOfferedCourse!.Course!.Title,
                    CourseCode = i.SemesterOfferedCourse.Course.Code,
                    SemesterName = i.Registration.Semester?.Name ?? string.Empty,
                    Status = i.Registration.Status == UniversityManagementSystem.Domain.Enums.RegistrationStatus.Submitted
                        ? "Pending"
                        : i.Registration.Status == UniversityManagementSystem.Domain.Enums.RegistrationStatus.Approved
                            ? i.Status == UniversityManagementSystem.Domain.Enums.RegistrationItemStatus.Registered ? "Approved" : "Waitlisted"
                            : i.Registration.Status.ToString(),
                    EnrollmentDate = i.Registration.CreatedAt,
                    ApprovalDate = i.Registration.ReviewedAt
                }));

            return rows.OrderByDescending(row => row.EnrollmentDate).ToList();
        }

        public async Task<IActionResult> ApproveReject(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null) return NotFound();
            return View("Enrollments/ApproveReject", enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEnrollment(int id)
        {
            await _enrollmentService.ApproveEnrollmentAsync(id);
            var user = await _userManagementService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Approve", "Enrollment", id, $"Approved enrollment ID {id}");
            return RedirectToAction(nameof(PendingEnrollments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEnrollment(int id, string reason)
        {
            await _enrollmentService.RejectEnrollmentAsync(id, reason);
            var user = await _userManagementService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Reject", "Enrollment", id, $"Rejected enrollment ID {id}: {reason}");
            return RedirectToAction(nameof(PendingEnrollments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            try
            {
                await _enrollmentService.DeleteEnrollmentAsync(id);
                var user = await _userManagementService.GetCurrentUserAsync();
                await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Delete", "Enrollment", id, $"Deleted enrollment ID {id}");
                TempData["Success"] = "Enrollment deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete enrollment: {ex.Message}";
            }
            return RedirectToAction(nameof(Enrollments));
        }
        #endregion

        // ===== NEW: USER MANAGEMENT =====
        #region User Management
        public async Task<IActionResult> Users(string role, string search, int page = 1, int pageSize = 10)
        {
            var result = await _userManagementService.GetPagedUsersAsync(role, search, page, pageSize);
            return View("Users/Index", result);
        }

        public IActionResult CreateUser()
        {
            ViewBag.Roles = new SelectList(new[] { "Admin", "Instructor", "Student" });
            return View("Users/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                // For students, use the StudentId/UGR as the login username
                if (string.Equals(dto.Role, "Student", StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrEmpty(dto.Username))
                {
                    // When creating via the generic user form for a student,
                    // the admin should supply the StudentId in Username.
                    ModelState.AddModelError(nameof(dto.Username),
                        "For Student role, the Username must be the Student UGR/StudentId.");
                    ViewBag.Roles = new SelectList(new[] { "Admin", "Instructor", "Student" });
                    return View("Users/Create", dto);
                }

                await _userManagementService.CreateUserAsync(dto);
                TempData["Success"] = "User created successfully.";
                return RedirectToAction(nameof(Users));
            }
            ViewBag.Roles = new SelectList(new[] { "Admin", "Instructor", "Student" });
            return View("Users/Create", dto);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManagementService.GetUserForEditAsync(id);
            if (user == null) return NotFound();
            return View("Users/Edit", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _userManagementService.UpdateUserAsync(dto);
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Users));
            }
            return View("Users/Edit", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            await _userManagementService.ActivateUserAsync(userId);
            var user = await _userManagementService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Activate", "User", null, $"Activated user {userId}");
            TempData["Success"] = "User activated.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            await _userManagementService.DeactivateUserAsync(userId);
            var user = await _userManagementService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(user.Id, $"{user.FirstName} {user.LastName}", "Admin", "Deactivate", "User", null, $"Deactivated user {userId}");
            TempData["Success"] = "User deactivated.";
            return RedirectToAction(nameof(Users));
        }
        #endregion

        // ===== NEW: ATTENDANCE OVERVIEW =====
        #region Attendance Overview
        public async Task<IActionResult> AttendanceOverview()
        {
            var stats = await _attendanceService.GetAttendanceStatsAsync();
            return View("Attendance/Overview", stats);
        }

        public async Task<IActionResult> AttendanceReports()
        {
            var reports = await _attendanceService.GetAllAttendanceReportsAsync();
            return View("Attendance/Reports", reports);
        }
        #endregion

        // ===== NEW: REPORTS & EXPORT =====
        #region Reports
        public IActionResult Reports()
        {
            return View("Reports/Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportReport(string reportType, string format, ExportFilterDto filter)
        {
            try
            {
                var fileBytes = await _reportService.GenerateReportAsync(reportType, format, filter);
                var isExcel = string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase);
                var extension = isExcel ? "xlsx" : "pdf";
                var contentType = isExcel
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "application/pdf";
                return File(fileBytes, contentType, $"Report_{reportType}_{DateTime.Now:yyyyMMdd}.{extension}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to export report: {ex.Message}";
                return RedirectToAction(nameof(Reports));
            }
        }
        #endregion

        // ===== NEW: AUDIT LOGS =====
        #region Audit Logs
        // NOTE: the filter parameters are deliberately named actionFilter /
        // entityFilter. A parameter named "action" collides with the MVC route
        // value "action" and would always bind to "AuditLogs", silently
        // filtering out every audit entry.
        public async Task<IActionResult> AuditLogs(
            string? searchText,
            string? actionFilter,
            string? entityFilter,
            DateTime? from,
            DateTime? to,
            int page = 1)
        {
            const int pageSize = 50;

            var (items, totalCount) = await _auditLogService.SearchAsync(
                searchText: searchText,
                action: actionFilter,
                entity: entityFilter,
                from: from,
                to: to,
                page: page,
                pageSize: pageSize
            );

            ViewBag.SearchText = searchText;
            ViewBag.SelectedAction = actionFilter;
            ViewBag.SelectedEntity = entityFilter;
            ViewBag.FromDate = from?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to?.ToString("yyyy-MM-dd");
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            // Distinct actions/entities for the filter dropdowns, read DB-side.
            ViewBag.ActionList = (await _auditLogService.GetDistinctActionsAsync()).ToList();
            ViewBag.EntityList = (await _auditLogService.GetDistinctEntitiesAsync()).ToList();

            return View("AuditLogs/Index", items);
        }

        public async Task<IActionResult> ExportAuditLogs(
            string? searchText,
            string? actionFilter,
            string? entityFilter,
            DateTime? from,
            DateTime? to)
        {
            var logs = await _auditLogService.GetAuditLogsAsync(
                searchText: searchText,
                action: actionFilter,
                entity: entityFilter,
                from: from,
                to: to
            );

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Date/Time,User,IP Address,Action,Entity,Entity ID,Description");
            foreach (var log in logs)
            {
                var details = (log.Details ?? string.Empty).Replace("\"", "\"\"");
                var userName = (log.UserName ?? string.Empty).Replace("\"", "\"\"");
                var ipAddress = (log.IpAddress ?? "-").Replace("\"", "\"\"");
                var logAction = (log.Action ?? string.Empty).Replace("\"", "\"\"");
                var logEntity = (log.Entity ?? string.Empty).Replace("\"", "\"\"");
                csv.AppendLine($"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{userName}\",\"{ipAddress}\",\"{logAction}\",\"{logEntity}\",\"{log.EntityId ?? 0}\",\"{details}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        public async Task<IActionResult> AuditLogDetails(int id)
        {
            var log = await _auditLogService.GetAuditLogByIdAsync(id);
            if (log == null) return NotFound();
            return View("AuditLogs/Details", log);
        }
        #endregion

        // ===== STUDENT PROMOTION =====
        #region Student Promotion
        public async Task<IActionResult> StudentPromotion()
        {
            var currentYear = await _context.AcademicYears
                .OrderByDescending(y => y.StartDate)
                .FirstOrDefaultAsync();
            if (currentYear == null)
            {
                TempData["Error"] = "No academic year configured. Please set up an academic year first.";
                return RedirectToAction(nameof(Dashboard));
            }

            var overview = await _context.AcademicGrades
                .Include(g => g.Sections)
                .OrderBy(g => g.PromotionOrder)
                .ToListAsync();

            var model = new PromotionOverviewDto
            {
                CurrentAcademicYearId = currentYear.Id,
                CurrentAcademicYearName = currentYear.Name,
                Sections = new List<PromotionSectionDto>(),
                TotalStudents = 0,
                TotalEligible = 0,
                TotalGraduating = 0
            };

            int totalStudents = 0;
            int totalEligible = 0;
            int totalGraduating = 0;
            int maxPromotionOrder = overview.Max(g => g.PromotionOrder);

            // Map each configured year to its class standing (entry year = Freshman).
            var standingMap = YearHelper.BuildStandingMap(overview.Select(g => g.PromotionOrder));

            foreach (var grade in overview)
            {
                var isGraduating = grade.PromotionOrder == maxPromotionOrder;

                foreach (var section in grade.Sections)
                {
                    var students = await _context.Students
                        .Where(s => s.AcademicSectionId == section.Id)
                        .ToListAsync();

                    int count = students.Count;
                    totalStudents += count;
                    totalEligible += count;
                    if (isGraduating)
                        totalGraduating += count;

                    model.Sections.Add(new PromotionSectionDto
                    {
                        GradeId = grade.Id,
                        YearName = grade.Name,
                        YearDisplayName = standingMap.TryGetValue(grade.PromotionOrder, out var standing)
                            ? standing
                            : grade.Name,
                        PromotionOrder = grade.PromotionOrder,
                        SectionName = section.Name,
                        StudentCount = count,
                        PendingSemester1 = count,
                        PendingSemester2 = count,
                        Status = count > 0 ? "Pending" : "No Students",
                        IsGraduating = isGraduating
                    });
                }
            }

            model.TotalStudents = totalStudents;
            model.TotalEligible = totalEligible;
            model.TotalGraduating = totalGraduating;

            ViewBag.AcademicYears = new SelectList(
                await _context.AcademicYears.OrderBy(y => y.StartDate).ToListAsync(),
                "Id", "Name", currentYear.Id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteSection(int gradeId, string sectionName, int academicYearId)
        {
            var grade = await _context.AcademicGrades
                .Include(g => g.Sections)
                .FirstOrDefaultAsync(g => g.Id == gradeId);

            if (grade == null)
            {
                TempData["Error"] = "Academic year not found.";
                return RedirectToAction(nameof(StudentPromotion));
            }

            var section = grade.Sections.FirstOrDefault(s => s.Name == sectionName);
            if (section == null)
            {
                TempData["Error"] = "Section not found.";
                return RedirectToAction(nameof(StudentPromotion));
            }

            var students = await _context.Students
                .Where(s => s.AcademicSectionId == section.Id)
                .ToListAsync();

            if (!students.Any())
            {
                TempData["Error"] = $"No students found in {grade.Name} - {section.Name}.";
                return RedirectToAction(nameof(StudentPromotion));
            }

            var nextGrade = await _context.AcademicGrades
                .FirstOrDefaultAsync(g => g.PromotionOrder == grade.PromotionOrder + 1);

            if (nextGrade == null)
            {
                TempData["Error"] = $"No higher year available. {grade.Name} is already the highest year.";
                return RedirectToAction(nameof(StudentPromotion));
            }

            var nextSection = await _context.AcademicSections
                .FirstOrDefaultAsync(s => s.GradeId == nextGrade.Id && s.Name == section.Name);

            if (nextSection == null)
            {
                nextSection = new AcademicSection
                {
                    Name = section.Name,
                    GradeId = nextGrade.Id,
                    InstructorId = section.InstructorId
                };
                _context.AcademicSections.Add(nextSection);
                await _context.SaveChangesAsync();
            }

            foreach (var student in students)
            {
                student.AcademicGradeId = nextGrade.Id;
                student.AcademicSectionId = nextSection.Id;
            }

            await _context.SaveChangesAsync();

            var user = await _userManagementService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(
                user?.Id ?? "",
                $"{user?.FirstName} {user?.LastName}",
                "Admin",
                "Promote",
                "Student Promotion",
                null,
                $"Promoted {students.Count} student(s) from {grade.Name} {section.Name} to {nextGrade.Name} {section.Name}.");

            TempData["Success"] = $"Promoted {students.Count} student(s) from {grade.Name} {section.Name} to {nextGrade.Name} {section.Name}.";
            return RedirectToAction(nameof(StudentPromotion));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteAll(string confirmCode, int academicYearId)
        {
            if (!string.Equals(confirmCode, "PROMOTE ALL", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Please type PROMOTE ALL exactly to confirm.";
                return RedirectToAction(nameof(StudentPromotion));
            }

            var academicYears = await _context.AcademicYears
                .OrderByDescending(y => y.StartDate)
                .ToListAsync();

            if (!academicYears.Any())
            {
                TempData["Error"] = "No academic year configured.";
                return RedirectToAction(nameof(StudentPromotion));
            }

            var targetYear = academicYears.First();
            if (academicYearId > 0)
            {
                targetYear = academicYears.FirstOrDefault(y => y.Id == academicYearId) ?? targetYear;
            }

            var grades = await _context.AcademicGrades
                .Include(g => g.Sections)
                .OrderBy(g => g.PromotionOrder)
                .ToListAsync();

            int maxPromotionOrder = grades.Max(g => g.PromotionOrder);
            int promotedSections = 0;
            int totalPromoted = 0;

            foreach (var grade in grades)
            {
                var isGraduating = grade.PromotionOrder == maxPromotionOrder;

                foreach (var section in grade.Sections)
                {
                    var students = await _context.Students
                        .Where(s => s.AcademicSectionId == section.Id)
                        .ToListAsync();

                    if (!students.Any())
                        continue;

                    var nextGrade = await _context.AcademicGrades
                        .FirstOrDefaultAsync(g => g.PromotionOrder == grade.PromotionOrder + 1);

                    if (nextGrade == null)
                        continue;

                    var nextSection = await _context.AcademicSections
                        .FirstOrDefaultAsync(s => s.GradeId == nextGrade.Id && s.Name == section.Name);

                    if (nextSection == null)
                    {
                        nextSection = new AcademicSection
                        {
                            Name = section.Name,
                            GradeId = nextGrade.Id,
                            InstructorId = section.InstructorId
                        };
                        _context.AcademicSections.Add(nextSection);
                        await _context.SaveChangesAsync();
                    }

                    foreach (var student in students)
                    {
                        student.AcademicGradeId = nextGrade.Id;
                        student.AcademicSectionId = nextSection.Id;
                    }

                    promotedSections++;
                    totalPromoted += students.Count;
                }
            }

            await _context.SaveChangesAsync();

            var adminUser = await _userManagementService.GetCurrentUserAsync();
            await _auditLogService.LogAsync(
                adminUser?.Id ?? "",
                $"{adminUser?.FirstName} {adminUser?.LastName}",
                "Admin",
                "Promote All",
                "Student Promotion",
                null,
                $"Bulk promoted {promotedSections} section(s) ({totalPromoted} students) for academic year '{targetYear.Name}'.");

            TempData["Success"] = $"Promoted {totalPromoted} student(s) across {promotedSections} section(s).";
            return RedirectToAction(nameof(StudentPromotion));
        }
        #endregion

        // ===== ACADEMIC HISTORY =====
        #region Academic History
        public async Task<IActionResult> AcademicHistory(int? academicYearId, int? gradeId, int? sectionId)
        {
            ViewBag.AcademicYears = new SelectList(await _context.AcademicYears.OrderBy(y => y.Name).ToListAsync(), "Id", "Name");
            ViewBag.AcademicGrades = new SelectList(await _context.AcademicGrades.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            ViewBag.Sections = new SelectList(
                sectionId.HasValue
                    ? await _context.AcademicSections.Where(s => s.GradeId == (gradeId ?? 0)).OrderBy(s => s.Name).ToListAsync()
                    : new List<AcademicSection>(),
                "Id", "Name");

            ViewBag.SelectedAcademicYearId = academicYearId;
            ViewBag.SelectedGradeId = gradeId;
            ViewBag.SelectedSectionId = sectionId;

            // If filters are selected, fetch student records
            var students = new List<StudentAcademicRecordDto>();
            if (academicYearId.HasValue && gradeId.HasValue && sectionId.HasValue)
            {
                // Get students enrolled in courses for this year/grade/section
                var courseGradeIds = await _context.CourseGrades
                    .Where(cg => cg.GradeId == gradeId.Value)
                    .Select(cg => cg.CourseId)
                    .ToListAsync();

                var enrollments = await _context.Enrollments
                    .Include(e => e.Student)
                    .ThenInclude(s => s.Department)
                    .Include(e => e.Course)
                    .Where(e => courseGradeIds.Contains(e.CourseId))
                    .ToListAsync();

                var groupedStudents = enrollments
                    .GroupBy(e => e.StudentId)
                    .Select(g => new StudentAcademicRecordDto
                    {
                        StudentId = g.Key,
                        StudentName = $"{g.First().Student.FirstName} {g.First().Student.MiddleName} {g.First().Student.LastName}",
                        StudentUgr = g.First().Student.StudentId,
                        DepartmentName = g.First().Student.Department?.Name ?? "N/A",
                        TotalCourses = g.Count(),
                        ApprovedEnrollments = g.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Approved),
                        PendingEnrollments = g.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Pending)
                    })
                    .OrderBy(s => s.StudentName)
                    .ToList();

                students = groupedStudents;
            }

            ViewBag.StudentRecords = students;
            return View("AcademicHistory");
        }
        #endregion
    }
}