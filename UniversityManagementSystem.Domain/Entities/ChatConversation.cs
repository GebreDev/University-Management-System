using UniversityManagementSystem.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Domain.Entities
{
    public class ChatConversation : BaseEntity
    {
        public string User1Id { get; set; } = string.Empty;
        public string User2Id { get; set; } = string.Empty;
        public string User1Role { get; set; } = string.Empty;
        public string User2Role { get; set; } = string.Empty;
        public string? LastMessage { get; set; } = string.Empty;
        public DateTime? LastMessageTime { get; set; }

        [ForeignKey("User1Id")]
        public virtual ApplicationUser User1 { get; set; } = null!;
        [ForeignKey("User2Id")]
        public virtual ApplicationUser User2 { get; set; } = null!;

        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}