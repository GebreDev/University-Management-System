using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? RelatedId { get; set; } = string.Empty;
        public string? LinkUrl { get; set; } = string.Empty;

        public virtual ApplicationUser User { get; set; } = null!;
    }
}