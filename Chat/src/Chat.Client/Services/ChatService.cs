using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client.Services;

public class ChatService: IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly IdentityStateProvider _identityStateProvider;
    private string? _userId;
    private string? _token;
    public event Func<string, string, string?, string?, Task>? OnMessageReceived;
    public event Func<string, string, Task>? OnMessageSeen;
    public event Func<string, Task>? OnFriendRemoved;

    public ChatService(IdentityStateProvider identityStateProvider)
    {
        _identityStateProvider = identityStateProvider;
    }

    public async Task InitializeGlobalConnectionAsync()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
            return;

        _userId = await _identityStateProvider.GetUserId();
        _token = await _identityStateProvider.GetToken();

        if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_userId))
            return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://192.168.1.78:5142/chathub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_token)!;
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string, string?, string?>("SendMessage", async (senderId, message, fileUrl, fileName) =>
        {
            if (OnMessageReceived != null)
                await OnMessageReceived(senderId, message, fileUrl, fileName);
        });

        _hubConnection.On<string, string>("MarkAsSeen", async (senderId, receiverId) =>
        {
            if (OnMessageSeen != null)
                await OnMessageSeen(senderId, receiverId);
        });
        
        _hubConnection.On<string>("FriendRemoved", async (removerId) =>
        {
            if (OnFriendRemoved != null)
                await OnFriendRemoved.Invoke(removerId);
        });

        await _hubConnection.StartAsync();
    }

    public async Task JoinRoomAsync(string friendId)
    {
        var me = await _identityStateProvider.GetUserId();
        await _hubConnection!.InvokeAsync("JoinRoom", me, friendId);
    }

    public async Task LeaveRoomAsync(string friendId)
    {
        var me = await _identityStateProvider.GetUserId();
        await _hubConnection!.InvokeAsync("LeaveRoom", me, friendId);
    }

    public async Task SendMessageAsync(string senderId, string receiverId, string message, string? fileUrl = null, string? fileName = null)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SendMessage", senderId, receiverId, message, fileUrl, fileName);
        }
    }

    public async Task MarkAsSeenAsync(string senderId, string receiverId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("MarkAsSeen", senderId, receiverId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
        _hubConnection = null;
    }
}