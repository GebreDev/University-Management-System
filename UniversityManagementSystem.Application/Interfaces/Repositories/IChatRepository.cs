using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IChatRepository : IRepository<ChatConversation>
    {
        Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string userId);
        Task<ChatConversation?> GetConversationAsync(string user1Id, string user2Id);
        Task<IEnumerable<ChatMessage>> GetMessagesAsync(int conversationId, int skip, int take);
        Task<ChatMessage> AddMessageAsync(ChatMessage message);
        Task<ChatMessage?> GetMessageByIdAsync(int messageId);
    }
}