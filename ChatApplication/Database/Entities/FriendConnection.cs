namespace Database.Entities;

public class FriendConnection
{
    public int Id { get; set; }
    public string? FromId { get; set; }
    public string? ToId { get; set; }
    
    public User? From { get; set; }
    public User? To { get; set; }
}