using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Hubs
{
    public class SignalRNotificationBroadcaster : INotificationBroadcaster
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationBroadcaster(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastNotificationAsync(string userId, int notificationId, string title, string message, string type, string createdAt)
        {
            try
            {
                await _hubContext.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", new
                {
                    id = notificationId,
                    title = title,
                    message = message,
                    type = type,
                    createdAt = createdAt,
                    isRead = false
                });
            }
            catch
            {
                // Best-effort broadcast; don't let SignalR failures bubble up
            }
        }
    }
}
