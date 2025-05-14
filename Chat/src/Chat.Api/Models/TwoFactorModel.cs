namespace Chat.Api.Models;

public class TwoFactorModel
{
    public string UserId { get; set; }= string.Empty;
    public string TwoFactorCode { get; set; } = string.Empty;
}