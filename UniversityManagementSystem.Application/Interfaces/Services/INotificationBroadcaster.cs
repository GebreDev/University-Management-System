using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Abstraction for broadcasting real-time notifications via SignalR.
    /// Implemented in the Web layer to avoid Application->Web dependency.
    /// </summary>
    public interface INotificationBroadcaster
    {
        Task BroadcastNotificationAsync(string userId, int notificationId, string title, string message, string type, string createdAt);
    }
}
