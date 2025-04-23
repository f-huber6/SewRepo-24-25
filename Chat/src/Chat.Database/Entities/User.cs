using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Chat.Database.Entities;

[Table("USERS")]
public class User: IdentityUser
{
    [Column("FIRST_NAME"), Required, StringLength(45)]
    public string FirstName { get; set; } = string.Empty;
    
    [Column("LAST_NAME"), Required, StringLength(45)]
    public string LastName { get; set; } = string.Empty;

    [Column("TWO_FACTOR_CODE"), StringLength(7)]
    public string? TwoFactorCode { get; set; } = string.Empty;
    
    public DateTime TwoFactorCodeExpiration { get; set; }
}