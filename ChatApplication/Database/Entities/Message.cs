using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("Messages")]
public class Message
{
    [Key, Column("MessageId"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId { get; set; }
    
    [Column("RoomName"), Required, StringLength(30)]
    public string? RoomName { get; set; }
    
    [Column("Content"), Required, StringLength(7000)]
    public string? Content { get; set; }
    
    [Column("Sender"), Required, StringLength(100)]
    public string? Sender { get; set; }
    
    [Column("Timestamp"), Required]
    public DateTime Timestamp { get; set; }
    
    public User? User { get; set; }
    
    public int UserId { get; set; }      
}