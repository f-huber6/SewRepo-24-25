using System.Security.Claims;

namespace ChatApplication.Hub;
using Microsoft.AspNetCore.SignalR;

public class ChatHub(ILogger<ChatHub> logger) : Hub
{
    private static readonly Dictionary<string, string> ConnectedUsers = new();

    private static readonly Dictionary<string, HashSet<string>> ChatRooms = new()
    {
        ["General"] = new HashSet<string>()
    };

    private readonly ILogger<ChatHub> _logger = logger;

    public async Task RegisterUser(string nickname)
    {
        var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (ConnectedUsers.ContainsKey(Context.ConnectionId) || ConnectedUsers.ContainsValue(nickname))
        {
            _logger.LogWarning("Registration failed: nickname already in use");
            await Clients.Caller.SendAsync("RegistrationFailed", "Nickname already in use");
            return;
        }

        ConnectedUsers[Context.ConnectionId] = nickname;
        _logger.LogInformation("User '{Nickname}' registered successfully with connection ID '{ConnectionId}'", nickname, Context.ConnectionId);
        await Clients.Caller.SendAsync("RegistrationSucceeded", nickname, ChatRooms.Keys);
    }

    public async Task CreateChatRoom(string roomName)
    {
        if (!ChatRooms.ContainsKey(roomName))
        {
            ChatRooms[roomName] = new HashSet<string>();
            _logger.LogInformation("Chat room '{RoomName}' created", roomName);
            await Clients.All.SendAsync("ChatRoomCreated", roomName);
        }
        else
        {
            _logger.LogWarning("Chat room '{RoomName}' already exists", roomName);
        }
    }

    public async Task DeleteChatRoom(string roomName)
    {
        if (ChatRooms.ContainsKey(roomName) && ChatRooms[roomName].Count == 0 && roomName != "General")
        {
            ChatRooms.Remove(roomName, out _);
            _logger.LogInformation("Chat room '{RoomName}' deleted", roomName);
            await Clients.All.SendAsync("ChatRoomDeleted", roomName);
        }
        else
        {
            _logger.LogWarning("Attempt to delete chat room '{RoomName}' failed (room might not be empty or is 'General')", roomName);
        }
    }

    public async Task UpdateUserList(string roomName)
    {
        if (ChatRooms.ContainsKey(roomName))
        {
            var userInRoom = ChatRooms[roomName].ToList();
            await Clients.Group(roomName).SendAsync("UpdateUserList", userInRoom);
        }
    }
    
    public async Task JoinChatRoom(string roomName)
    {
        if (ConnectedUsers.TryGetValue(Context.ConnectionId, out var nickname))
        {
            if (ChatRooms.ContainsKey(roomName))
            {
                ChatRooms[roomName].Add(nickname);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                _logger.LogInformation("User '{Nickname}' joined chat room '{RoomName}'", nickname, roomName);
                await Clients.Group(roomName).SendAsync("UserJoined", nickname);
                await UpdateUserList(roomName);
            }
            else
            {
                _logger.LogWarning("User '{Nickname}' attempted to join non-existent chat room '{RoomName}'", nickname, roomName);
            }
        }
    }

    public async Task LeaveChatRoom(string roomName)
    {
        if (ConnectedUsers.TryGetValue(Context.ConnectionId, out var nickname))
        {
            if (ChatRooms.ContainsKey(roomName))
            {
                ChatRooms[roomName].Remove(nickname);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
                _logger.LogInformation("User '{Nickname}' left chat room '{RoomName}'", nickname, roomName);
                await Clients.Group(roomName).SendAsync("UserLeft", nickname);
                await UpdateUserList(roomName);
            }
            else
            {
                _logger.LogWarning("User '{Nickname}' attempted to leave non-existent chat room '{RoomName}'", nickname, roomName);
            }
        }
    }
    
    public async Task LogoutUser()
    {
        if (ConnectedUsers.Remove(Context.ConnectionId, out var nickname))
        {
            foreach (var room in ChatRooms)
            {
                if (room.Value.Contains(nickname))
                {
                    room.Value.Remove(nickname);
                    await Clients.Group(room.Key).SendAsync("UserLeft", nickname);
                }
            }
            
            _logger.LogInformation("User '{Nickname}' logged out", nickname);
            await Clients.Caller.SendAsync("UserLoggedOut", nickname);
        }
    }

    public async Task SendMessage(string roomName, string message)
    {
        if (ConnectedUsers.TryGetValue(Context.ConnectionId, out var nickname))
        {
            _logger.LogInformation("User '{Nickname}' sent message to '{RoomName}': {Message}", nickname, roomName, message);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", nickname, message);
        }
        else
        {
            _logger.LogError("Message send failed: User with connection ID '{ConnectionId}' not found", Context.ConnectionId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if(ConnectedUsers.Remove(Context.ConnectionId, out var nickname))
        {
            foreach(var room in ChatRooms)
            {
                if(room.Value.Contains(nickname))
                {
                    room.Value.Remove(nickname);
                    await Clients.Group(room.Key).SendAsync("UserLeft", nickname);
                }
            }
            
            _logger.LogInformation("User '{Nickname}' disconnected", nickname);
        }
        
        else
        {
            _logger.LogWarning("Unknown user with connection ID '{ConnectionId}' disconnected", Context.ConnectionId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "An error occurred during disconnection of user '{ConnectionId}'", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}