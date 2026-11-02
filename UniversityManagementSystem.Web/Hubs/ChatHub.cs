using UniversityManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // Lightweight presence tracker: user id -> last seen tick (in-memory).
        private static readonly ConcurrentDictionary<string, DateTime> _presence = new();
        public static bool IsOnline(string userId)
            => !string.IsNullOrEmpty(userId) && _presence.ContainsKey(userId);

        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conv-{conversationId}");
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv-{conversationId}");
        }

        public async Task NotifyNewMessage(int conversationId)
        {
            // Notify all other users in the conversation group that new message was sent
            await Clients.OthersInGroup($"conv-{conversationId}").SendAsync("NewMessage", conversationId);
        }

        public async Task SendTyping(int conversationId, bool isTyping)
        {
            await Clients.OthersInGroup($"conv-{conversationId}").SendAsync("UserTyping", Context.UserIdentifier, isTyping);
        }

        public async Task MarkAsRead(int messageId)
        {
            // SignalR hub invocations do not have an ambient HttpContext, so use the
            // authenticated caller's identifier instead of IUserManagementService.
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
                return;
            await _chatService.MarkAsReadAsync(messageId, userId);
            await Clients.Caller.SendAsync("MessageRead", messageId);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
                _presence[userId] = DateTime.UtcNow;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
                _presence.TryRemove(userId, out _);
            await base.OnDisconnectedAsync(exception);
        }
    }
}