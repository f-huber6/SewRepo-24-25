using System.Net.Http.Json;

namespace Chat.Client.Services;

public class UnreadMessageService
{
    private readonly HttpClient _httpClient;
    public List<UnreadCountModel> UnreadCounts { get; private set; } = new();
    public event Func<Task>? OnUnreadCountsChanged;
    
    public UnreadMessageService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task LoadAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<List<UnreadCountModel>>("api/chat/unread-counts") ?? new();
            UnreadCounts = data;
        }
        
        catch (Exception ex)
        {
            return;
        }
        
        if(OnUnreadCountsChanged is not null)
            await OnUnreadCountsChanged.Invoke();
    }

    public int GetTotalUnread() => UnreadCounts.Sum(s => s.UnreadCount);

    public int GetUnreadCountForFriend(string friendId)
        => UnreadCounts.FirstOrDefault(s => s.FriendId == friendId)?.UnreadCount ?? 0;
}

public class UnreadCountModel
{
    public string FriendId { get; set; } = "";
    public int UnreadCount { get; set; } = 0;
}