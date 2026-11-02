using System;
using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Chat
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string OtherUserId { get; set; }
        public string OtherUserFullName { get; set; }
        public string OtherUserRole { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}