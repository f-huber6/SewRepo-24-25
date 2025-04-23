namespace Chat.Api.Hub;
using Microsoft.AspNetCore.SignalR;

public class FriendHub: Hub
{
    public async Task NotifyFriendRequest(string userId)
    {
        await Clients.User(userId).SendAsync("ReceiveFriendRequest");
    }
}