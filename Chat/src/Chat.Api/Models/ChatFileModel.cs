namespace Chat.Api.Models;

public class ChatFileModel
{
    public string FileName { get; set; } = "";
    public string FileUrl { get; set; } = "";
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}