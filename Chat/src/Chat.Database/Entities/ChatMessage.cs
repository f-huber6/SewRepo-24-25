using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Database.Entities;

[Table("ChatMessages")]
public class ChatMessage
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required, Column("SENDER_ID"), StringLength(50)]
    public string SenderId { get; set; } = string.Empty;
    
    [Required, Column("RECEIVER_ID"), StringLength(50)]
    public string ReceiverId { get; set; } = string.Empty;
    
    [Column("TEXT"), Required, StringLength(4000)]
    public string Text { get; set; } = string.Empty;
    
    [Column("CREATED_AT")]
    public DateTime SentAt { get; set; } = DateTime.Now;
    
    [Column("IS_SEEN")]
    public bool IsSeen { get; set; } = false;
    
    [Column("SEEN_AT")]
    public DateTime? SeenAt { get; set; } = null;
    
    public ICollection<ChatFile>? ChatFiles { get; set; } = new List<ChatFile>();
}