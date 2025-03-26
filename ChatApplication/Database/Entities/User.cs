using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Database.Entities;

[Table("Users")]
public class User: IdentityUser
{
    [Key, Column("Id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("UserName"), Required, StringLength(40)]
    public override string? UserName { get; set; } = string.Empty;

    public ICollection<Message>? Messages { get; set; } = new List<Message>();
}