using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        public async Task SendNotification(string userId, string title, string message, string type = "")
        {
            await Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", new
            {
                title = title,
                message = message,
                type = type,
                timestamp = System.DateTime.UtcNow.ToString("o")
            });
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}