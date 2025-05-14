using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client.Services;

public class FriendService
{
    private readonly IdentityStateProvider _identityStateProvider;
    private HubConnection? _hubConnection;
    public event Func<Task>? OnFriendRequestsChanged;

    public FriendService(IdentityStateProvider identityStateProvider)
    {
        _identityStateProvider = identityStateProvider;
    }

    public async Task InitializeAsync()
    {
        var token = await _identityStateProvider.GetToken();
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://192.168.1.78:5142/friendHub", opt =>
            {
                opt.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On("ReceiveFriendRequest", async () =>
        {
            if (OnFriendRequestsChanged is not null)
                await OnFriendRequestsChanged.Invoke();
        });

        await _hubConnection.StartAsync();
    }

    public async Task NotifyFriendRequest(string userId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("NotifyFriendRequest", userId);
        }
    }
}