using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Notifications;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notifRepo;
        private readonly IMapper _mapper;
        private readonly INotificationBroadcaster _broadcaster;

        public NotificationService(INotificationRepository notifRepo, IMapper mapper, INotificationBroadcaster broadcaster)
        {
            _notifRepo = notifRepo;
            _mapper = mapper;
            _broadcaster = broadcaster;
        }

        public async Task SendNotificationAsync(string userId, string title, string message, string type = "", string relatedId = "", string linkUrl = "")
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Type = type,
                RelatedId = relatedId,
                LinkUrl = linkUrl
            };
            await _notifRepo.AddAsync(notification);
            await _notifRepo.SaveChangesAsync();

            // Broadcast real-time notification via SignalR (best-effort)
            await _broadcaster.BroadcastNotificationAsync(
                userId,
                notification.Id,
                title,
                message,
                type,
                notification.CreatedAt.ToString("o")
            );
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false)
        {
            var notifications = await _notifRepo.GetByUserIdAsync(userId, unreadOnly);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notif = await _notifRepo.GetByIdAsync(notificationId);
            if (notif != null && notif.UserId == userId && !notif.IsRead)
            {
                notif.IsRead = true;
                notif.ReadAt = DateTime.UtcNow;
                await _notifRepo.UpdateAsync(notif);
                await _notifRepo.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _notifRepo.GetByUserIdAsync(userId, true);
            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
            }
            await _notifRepo.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var notifications = await _notifRepo.GetByUserIdAsync(userId, true);
            return notifications.Count();
        }

        public async Task DeleteNotificationAsync(int id)
        {
            var notif = await _notifRepo.GetByIdAsync(id);
            if (notif != null)
            {
                _notifRepo.Delete(notif);
                await _notifRepo.SaveChangesAsync();
            }
        }
    }
}