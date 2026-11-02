using System;

namespace UniversityManagementSystem.Application.DTOs.Chat
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderProfilePictureUrl { get; set; } = string.Empty;
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentType { get; set; } // image, document, etc.
    }
}