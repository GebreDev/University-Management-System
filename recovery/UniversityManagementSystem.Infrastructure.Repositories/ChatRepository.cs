using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class ChatRepository : GenericRepository<ChatConversation>, IChatRepository, IRepository<ChatConversation>
{
	public ChatRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string userId)
	{
		return await (from c in _dbSet.Where((ChatConversation c) => c.User1Id == userId || c.User2Id == userId).Include((ChatConversation c) => c.Messages)
			orderby c.LastMessageTime descending
			select c).ToListAsync();
	}

	public async Task<ChatConversation?> GetConversationAsync(string user1Id, string user2Id)
	{
		return await _dbSet.FirstOrDefaultAsync((ChatConversation c) => (c.User1Id == user1Id && c.User2Id == user2Id) || (c.User1Id == user2Id && c.User2Id == user1Id));
	}

	public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(int conversationId, int skip, int take)
	{
		return await (from m in _context.ChatMessages
			where m.ConversationId == conversationId
			orderby m.SentAt descending
			select m).Skip(skip).Take(take).ToListAsync();
	}

	public async Task<ChatMessage> AddMessageAsync(ChatMessage message)
	{
		await _context.ChatMessages.AddAsync(message);
		return message;
	}

	public async Task<ChatMessage?> GetMessageByIdAsync(int messageId)
	{
		return await _context.ChatMessages.FirstOrDefaultAsync((ChatMessage m) => m.Id == messageId);
	}
}
