namespace Chat.Api.Models;

public class UnreadCountModel
{
    public string FriendId { get; set; } = "";
    public int UnreadCount { get; set; } = 0;
}