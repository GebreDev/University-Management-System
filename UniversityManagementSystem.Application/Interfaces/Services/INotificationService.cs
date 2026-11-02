using UniversityManagementSystem.Application.DTOs.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string title, string message, string type = "", string relatedId = "", string linkUrl = "");
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task DeleteNotificationAsync(int id);
    }
}