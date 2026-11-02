using UniversityManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRegistrationOfferingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminRegistrationOfferingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int semesterId)
        {
            var semester = await _context.Semesters
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(s => s.Id == semesterId);

            if (semester == null)
                return NotFound();

            var departments = await _context.Departments.ToListAsync();
            var courses = await _context.Courses
                .Include(c => c.Department)
                .ToListAsync();

            ViewBag.SemesterId = semesterId;
            ViewBag.SemesterName = semester.Name;
            ViewBag.Departments = departments;
            ViewBag.Courses = courses;
            ViewBag.RegisteredCounts = await _context.RegistrationItems
                .Where(i => i.Registration!.SemesterId == semesterId
                    && i.Status == UniversityManagementSystem.Domain.Enums.RegistrationItemStatus.Registered)
                .GroupBy(i => i.SemesterOfferedCourseId)
                .Select(group => new { OfferingId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(group => group.OfferingId, group => group.Count);

            return View(await _context.SemesterOfferedCourses
                .Where(o => o.SemesterId == semesterId)
                .Include(o => o.Course)
                .Include(o => o.Department)
                .ToListAsync());
        }
    }
}
