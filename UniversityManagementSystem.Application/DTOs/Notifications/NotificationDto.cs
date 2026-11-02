using System;

namespace UniversityManagementSystem.Application.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string Type { get; set; }
        public string RelatedId { get; set; }
        public string LinkUrl { get; set; }
    }
}