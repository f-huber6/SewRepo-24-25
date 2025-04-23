namespace Chat.Api.Models;

public class ChatMessageModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SenderId { get; set; } = "";
    public string ReceiverId { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsSeen { get; set; } = false;
    public DateTime? SeenAt { get; set; } = null;
    public List<ChatFileModel>? Files { get; set; } = new List<ChatFileModel>();
}