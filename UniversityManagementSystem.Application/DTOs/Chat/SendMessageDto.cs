using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Chat
{
    public class SendMessageDto
    {
        public int? ConversationId { get; set; } // optional, if null we create new

        [Required]
        public string RecipientId { get; set; }

        [Required]
        public string Content { get; set; }

        public string? AttachmentBase64 { get; set; }
        public string? AttachmentFileName { get; set; }
        public string? AttachmentType { get; set; }
    }
}