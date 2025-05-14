using System.Net.Http.Json;

namespace Chat.Client.Services;

public class FriendRequestService
{
    private readonly HttpClient _httpClient;
    public List<FriendRequestModel> FriendRequests { get; private set; } = new();
    public event Func<Task>? OnFriendRequestsChanged;
    
    public FriendRequestService(HttpClient httpClient) => _httpClient = httpClient;
    
    public async Task LoadAsync()
    {
        var data = await _httpClient.GetFromJsonAsync<List<FriendRequestModel>>("api/users/pending-requests") ?? new();
        FriendRequests = data;
        
        if(OnFriendRequestsChanged is not null)
            await OnFriendRequestsChanged.Invoke();
    }
}

public class FriendRequestModel
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public string? UserName { get; set; } = "";
}