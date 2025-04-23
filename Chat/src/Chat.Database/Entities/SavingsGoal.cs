using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Database.Entities;

[Table("SAVINGS_GOALS")]
public class SavingsGoal
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public User? User { get; set; }
    
    [Required, Column("USER_ID"), StringLength(50)]
    public string UserId { get; set; } = string.Empty;
    
    [Required, StringLength(60), Column("TITLE")]
    public string Title { get; set; } = "New Savings";
    
    [Required, Column("TARGET_AMOUNT")]
    public decimal TargetAmount { get; set; }
    
    [Column("CURRENT_AMOUNT")]
    public decimal CurrentAmount { get; set; } = 0;
    
    [Column("TARGET_DATE")]
    public DateTime? TargetDate { get; set; }
    
    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}