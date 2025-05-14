using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Database.Entities;

[Table("FriendShips")]
public class FriendShip
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required, Column("USER_ID"), StringLength(50)]
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
    
    [Required, Column("FRIEND_ID"), StringLength(50)]
    public string FriendId { get; set; } = string.Empty;
    public User? Friend { get; set; }
    
    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}