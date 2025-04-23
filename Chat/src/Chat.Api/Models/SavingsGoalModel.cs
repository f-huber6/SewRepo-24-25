namespace Chat.Api.Models;

public class SavingsGoalModel
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? TargetDate { get; set; }
}