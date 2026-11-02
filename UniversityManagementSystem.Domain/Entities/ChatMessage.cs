using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? AttachmentPath { get; set; } = string.Empty;
        public string? AttachmentType { get; set; } = string.Empty;

        public virtual ChatConversation Conversation { get; set; } = null!;
        public virtual ApplicationUser Sender { get; set; } = null!;
    }
}