namespace Chat.Api.Models;

public class RegisterModel
{
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
    public required string PhoneNumber { get; set; } = string.Empty;
    public required string UserName { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
}