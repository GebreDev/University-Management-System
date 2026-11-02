using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Chat;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
	public class ChatService : IChatService
	{
		private readonly IChatRepository _chatRepo;
		private readonly IUserManagementService _userService;
		private readonly IFileService _fileService;
		private readonly IMapper _mapper;

		public ChatService(IChatRepository chatRepo, IUserManagementService userService, IFileService fileService, IMapper mapper)
		{
			_chatRepo = chatRepo;
			_userService = userService;
			_fileService = fileService;
			_mapper = mapper;
		}

		public async Task SendMessageAsync(SendMessageDto dto, string senderId)
		{
			if (dto == null) throw new ArgumentNullException(nameof(dto));
			if (string.IsNullOrWhiteSpace(senderId)) throw new ArgumentNullException(nameof(senderId));
			if (string.IsNullOrWhiteSpace(dto.RecipientId)) throw new ArgumentException("RecipientId is required.");

			var conversation = await _chatRepo.GetConversationAsync(senderId, dto.RecipientId);
			if (conversation == null)
			{
				var sender = await _userService.GetUserByIdAsync(senderId);
				var recipient = await _userService.GetUserByIdAsync(dto.RecipientId);
				var senderRoles = await _userService.GetUserRolesAsync(senderId);
				var recipientRoles = await _userService.GetUserRolesAsync(dto.RecipientId);
				conversation = new ChatConversation
				{
					User1Id = senderId,
					User2Id = dto.RecipientId,
					User1Role = senderRoles != null && senderRoles.Count > 0 ? senderRoles[0] : "User",
					User2Role = recipientRoles != null && recipientRoles.Count > 0 ? recipientRoles[0] : "User",
					LastMessage = dto.Content ?? string.Empty,
					LastMessageTime = DateTime.UtcNow
				};
				await _chatRepo.AddAsync(conversation);
				await _chatRepo.SaveChangesAsync();
			}

			// Guard: ensure conversation.Id is valid after creation
			if (conversation.Id == 0)
			{
				throw new InvalidOperationException("Failed to create conversation: conversation ID is 0.");
			}

			// Only call SaveFileAsync when an actual attachment is provided
			string attachmentPath = string.Empty;
			if (!string.IsNullOrEmpty(dto.AttachmentBase64))
			{
				var savedPath = await _fileService.SaveFileAsync(dto.AttachmentBase64, "chat", dto.AttachmentFileName);
				attachmentPath = savedPath ?? string.Empty;
			}

			var message = new ChatMessage
			{
				ConversationId = conversation.Id,
				SenderId = senderId,
				Content = dto.Content ?? string.Empty,
				SentAt = DateTime.UtcNow,
				IsRead = false,
				AttachmentPath = attachmentPath,
				AttachmentType = dto.AttachmentType ?? string.Empty
			};
			await _chatRepo.AddMessageAsync(message);
			conversation.LastMessage = dto.Content ?? (conversation.LastMessage ?? string.Empty);
			conversation.LastMessageTime = DateTime.UtcNow;            await _chatRepo.UpdateAsync(conversation);
			await _chatRepo.SaveChangesAsync();
		}

		public async Task<IEnumerable<ConversationDto>> GetConversationsAsync(string userId)
		{
			var convs = await _chatRepo.GetUserConversationsAsync(userId);
			var result = new List<ConversationDto>();
			foreach (var conversation in convs)
			{
				var otherUserId = conversation.User1Id == userId ? conversation.User2Id : conversation.User1Id;
				var otherUser = await _userService.GetUserByIdAsync(otherUserId);
				var otherRoles = await _userService.GetUserRolesAsync(otherUserId);
				var dto = _mapper.Map<ConversationDto>(conversation);
				dto.OtherUserId = otherUserId;
				dto.OtherUserFullName = otherUser == null ? "Unknown user" : $"{otherUser.FirstName} {otherUser.LastName}";
				dto.OtherUserRole = otherRoles.FirstOrDefault() ?? "User";
				dto.ProfilePictureUrl = otherUser?.ProfilePictureUrl ?? string.Empty;
				dto.UnreadCount = conversation.Messages.Count(m => m.SenderId != userId && !m.IsRead);
				result.Add(dto);
			}
			return result;
		}

		public async Task<ConversationDto> GetConversationByIdAsync(int conversationId)
		{
			var conv = await _chatRepo.GetByIdAsync(conversationId);
			return _mapper.Map<ConversationDto>(conv);
		}

		public async Task<IEnumerable<MessageDto>> GetMessagesAsync(int conversationId, int skip, int take)
		{
			var msgs = await _chatRepo.GetMessagesAsync(conversationId, skip, take);
			var result = _mapper.Map<IEnumerable<MessageDto>>(msgs).ToList();

			// The repository returns messages newest-first (for pagination); flip to
			// chronological order so the UI renders oldest at the top.
			result.Reverse();

			// Populate sender display names + avatars for the message bubbles.
			var senderIds = result.Select(m => m.SenderId).Distinct().ToList();
			var senders = new Dictionary<string, ApplicationUser?>();
			foreach (var sid in senderIds)
			{
				senders[sid] = await _userService.GetUserByIdAsync(sid);
			}
			foreach (var m in result)
			{
				if (senders.TryGetValue(m.SenderId, out var sender) && sender != null)
				{
					m.SenderName = $"{sender.FirstName} {sender.LastName}";
					m.SenderProfilePictureUrl = sender.ProfilePictureUrl ?? string.Empty;
				}
				else
				{
					m.SenderName = "Unknown";
				}
			}

			return result;
		}

		public async Task MarkAsReadAsync(int messageId, string userId)
		{
			var message = await _chatRepo.GetMessageByIdAsync(messageId);
			if (message != null && !message.IsRead)
			{
				message.IsRead = true;
				message.ReadAt = DateTime.UtcNow;
				await _chatRepo.SaveChangesAsync();
			}
		}
	}
}
