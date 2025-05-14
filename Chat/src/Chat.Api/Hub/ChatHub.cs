using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Chat.Database.Context;

namespace Chat.Api.Hub;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly ApplicationContext _context;

    public ChatHub(ApplicationContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string senderId, string receiverId, string message, string? fileUrl, string? fileName)
    {
        await Clients.Group(receiverId)
            .SendAsync("SendMessage", senderId, message, fileUrl, fileName);
    }

    public async Task MarkAsSeen(string originalSenderId, string readerId)
    {
        await Clients.Group(originalSenderId)
            .SendAsync("MarkAsSeen", originalSenderId, readerId);
    }
    
    public async Task NotifyFriendRemoved(string removerId, string removedId)
    {
        await Clients.User(removedId).SendAsync("FriendRemoved", removerId);
    }

    public Task JoinRoom(string userId, string friendId)
    {
        var roomName = GetRoomName(userId, friendId);
        return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    public Task LeaveRoom(string userId, string friendId)
    {
        var roomName = GetRoomName(userId, friendId);
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }
    private string GetRoomName(string userId1, string userId2)
    {
        var sorted = new[] { userId1, userId2 }.OrderBy(x => x).ToArray();
        return $"room_{sorted[0]}_{sorted[1]}";
    }
}

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}