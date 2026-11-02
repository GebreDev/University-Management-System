using UniversityManagementSystem.Application.DTOs.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task SendMessageAsync(SendMessageDto dto, string senderId);
        Task<IEnumerable<ConversationDto>> GetConversationsAsync(string userId);
        Task<ConversationDto> GetConversationByIdAsync(int conversationId);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(int conversationId, int skip, int take);
        Task MarkAsReadAsync(int messageId, string userId);
    }
}