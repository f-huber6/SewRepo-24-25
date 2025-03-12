using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("Users")]
public class User
{
    [Key, Column("Id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("Username"), Required, StringLength(40)]
    public string? Username { get; set; }
    
    [Column("Password"), Required, StringLength(30)]
    public string? Password { get; set; }

    public ICollection<Message>? Messages { get; set; } = new List<Message>();
}