using UniversityManagementSystem.Application.DTOs.Chat;
using UniversityManagementSystem.Application.DTOs.Common;
using UniversityManagementSystem.Application.DTOs.UserManagement;
using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUserManagementService _userService;

        public ChatController(IChatService chatService, IUserManagementService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userService.GetCurrentUserAsync();
            var conversations = await _chatService.GetConversationsAsync(user.Id);
            var allUsers = await _userService.GetPagedUsersAsync(null, null, 1, 1000);
            ViewBag.CurrentUserId = user.Id;
            ViewBag.CurrentUserRole = await _userService.GetUserRoleAsync();
            ViewBag.AvailableUsers = BuildAvailableUsers(allUsers, user.Id);
            ViewBag.Contacts = BuildContacts(conversations, allUsers, user.Id);
            return View(conversations);
        }

        public async Task<IActionResult> Conversation(int conversationId, int skip = 0, int take = 50)
        {
            var user = await _userService.GetCurrentUserAsync();
            // Only conversation participants may read its messages.
            var conversations = await _chatService.GetConversationsAsync(user.Id);
            var isParticipant = conversations.Any(c => c.Id == conversationId);
            if (!isParticipant)
                return Forbid();

            var conversation = conversations.First(c => c.Id == conversationId);

            var messages = (await _chatService.GetMessagesAsync(conversationId, skip, take)).ToList();

            // Mark all received messages as read now that the user opened the chat.
            foreach (var msg in messages.Where(m => m.SenderId != user.Id && !m.IsRead))
            {
                await _chatService.MarkAsReadAsync(msg.Id, user.Id);
            }

            ViewBag.Conversation = conversation;
            ViewBag.CurrentUserId = user.Id;
            ViewBag.Skip = skip;
            ViewBag.Take = take;
            return PartialView("_ConversationMessages", messages);
        }

        [HttpGet]
        public IActionResult IsOnline(string userId)
        {
            return Json(new { online = !string.IsNullOrEmpty(userId) && Hubs.ChatHub.IsOnline(userId) });
        }

        private static List<SelectListItem> BuildAvailableUsers(PagedResult<UserListDto> allUsers, string currentUserId)
        {
            return allUsers.Data
                .Where(u => !string.Equals(u.Id, currentUserId, StringComparison.OrdinalIgnoreCase))
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FullName} ({u.Role})"
                }).ToList();
        }

        /// <summary>
        /// Builds the full contact list shown in the Messages sidebar: every registered
        /// user (Admins, Instructors and Students) with their role, merged with the
        /// last-message preview from any existing conversation.
        /// </summary>
        private static List<ChatContactDto> BuildContacts(
            IEnumerable<ConversationDto> conversations,
            PagedResult<UserListDto> allUsers,
            string currentUserId)
        {
            var conversationByUser = conversations
                .GroupBy(c => c.OtherUserId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(c => c.LastMessageTime).First(),
                    StringComparer.OrdinalIgnoreCase);

            var contacts = allUsers.Data
                .Where(u => !string.Equals(u.Id, currentUserId, StringComparison.OrdinalIgnoreCase))
                .Select(u =>
                {
                    conversationByUser.TryGetValue(u.Id, out var conv);
                    return new ChatContactDto
                    {
                        UserId = u.Id,
                        FullName = u.FullName,
                        Role = u.Role,
                        ProfilePictureUrl = conv?.ProfilePictureUrl ?? string.Empty,
                        ConversationId = conv?.Id,
                        LastMessage = conv?.LastMessage,
                        LastMessageTime = conv?.LastMessageTime,
                        UnreadCount = conv?.UnreadCount ?? 0
                    };
                })
                .ToList();

            // Keep conversations with users who are no longer in the user list (e.g. deleted users).
            var matchedUserIds = new HashSet<string>(contacts.Select(c => c.UserId), StringComparer.OrdinalIgnoreCase);
            foreach (var conv in conversationByUser.Values.Where(c => !matchedUserIds.Contains(c.OtherUserId)))
            {
                contacts.Add(new ChatContactDto
                {
                    UserId = conv.OtherUserId,
                    FullName = conv.OtherUserFullName,
                    Role = conv.OtherUserRole,
                    ProfilePictureUrl = conv.ProfilePictureUrl,
                    ConversationId = conv.Id,
                    LastMessage = conv.LastMessage,
                    LastMessageTime = conv.LastMessageTime,
                    UnreadCount = conv.UnreadCount
                });
            }

            return contacts
                .OrderByDescending(c => c.LastMessageTime)
                .ThenBy(c => c.FullName)
                .ToList();
        }

        public class ChatContactDto
        {
            public string UserId { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public string ProfilePictureUrl { get; set; } = string.Empty;
            public int? ConversationId { get; set; }
            public string? LastMessage { get; set; }
            public DateTime? LastMessageTime { get; set; }
            public int UnreadCount { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid message data. " + string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Message content cannot be empty.");

            var user = await _userService.GetCurrentUserAsync();
            try
            {
                await _chatService.SendMessageAsync(dto, user.Id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableUsers()
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var allUsers = await _userService.GetPagedUsersAsync(null, null, 1, 1000);
            return Json(BuildAvailableUsers(allUsers, currentUser.Id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.RecipientId))
                return BadRequest("Recipient is required.");

            if (string.IsNullOrWhiteSpace(request?.InitialMessage))
                return BadRequest("Initial message is required.");

            var user = await _userService.GetCurrentUserAsync();

            // Send an initial message to start the conversation
            var dto = new SendMessageDto
            {
                RecipientId = request.RecipientId,
                Content = request.InitialMessage,
                ConversationId = null
            };

            try
            {
                await _chatService.SendMessageAsync(dto, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to start conversation: {ex.InnerException?.Message ?? ex.Message}");
            }

            // Get the conversation that was just created/found
            var conversations = await _chatService.GetConversationsAsync(user.Id);
            var conversation = conversations
                .Where(c => string.Equals(c.OtherUserId, request.RecipientId, System.StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.LastMessageTime)
                .FirstOrDefault();

            if (conversation == null)
            {
                // Fallback: try to find any recent conversation with this recipient
                conversation = conversations
                    .Where(c => string.Equals(c.OtherUserId, request.RecipientId, System.StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
            }

            if (conversation == null)
                return BadRequest("Failed to create conversation. Please try again.");

            return Ok(new { conversationId = conversation.Id });
        }

        public class StartConversationRequest
        {
            public string? RecipientId { get; set; }
            public string? InitialMessage { get; set; }
        }
    }
}
