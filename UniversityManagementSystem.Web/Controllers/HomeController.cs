using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniversityManagementSystem.Web.Models;

namespace UniversityManagementSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserManagementService? _userService;

        public HomeController(ILogger<HomeController> logger, IUserManagementService? userService = null)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true && _userService != null)
            {
                var role = await _userService.GetUserRoleAsync();
                if (!string.IsNullOrEmpty(role))
                {
                    switch (role.ToLower())
                    {
                        case "admin":
                            return RedirectToAction("Dashboard", "Admin");
                        case "instructor":
                            return RedirectToAction("Dashboard", "Instructor");
                        case "student":
                            return RedirectToAction("Dashboard", "Student");
                    }
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}