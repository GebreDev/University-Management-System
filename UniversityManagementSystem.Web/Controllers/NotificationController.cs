using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IUserManagementService _userService;

        public NotificationController(INotificationService notificationService, IUserManagementService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userService.GetCurrentUserAsync();
            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var user = await _userService.GetCurrentUserAsync();
            await _notificationService.MarkAsReadAsync(id, user.Id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userService.GetCurrentUserAsync();
            await _notificationService.MarkAllAsReadAsync(user.Id);
            return Ok();
        }

        public async Task<IActionResult> GetUnreadCount()
        {
            var user = await _userService.GetCurrentUserAsync();
            var count = await _notificationService.GetUnreadCountAsync(user.Id);
            return Json(new { count });
        }
    }
}