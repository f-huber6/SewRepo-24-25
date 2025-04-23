using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Database.Entities;

[Table("Chatfiles")]
public class ChatFile
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required, Column("FILE_NAME"), StringLength(255)]
    public string? FileName { get; set; }
    
    [Required, Column("FILE_URL"), StringLength(1000)]
    public string? FileUrl { get; set; }
    
    [Column("CONTENT_TYPE"), StringLength(100)]
    public string? ContentType { get; set; }
    
    [Column("FILE_SIZE")]
    public long? FileSize { get; set; }
    
    [Column("UPLOADED_AT")]
    public DateTime? UploadedAt { get; set; } = DateTime.Now;
    
    [Required, StringLength(200)]
    public string? ChatMessageId { get; set; } = string.Empty;
    
    public ChatMessage? ChatMessage { get; set; }
}