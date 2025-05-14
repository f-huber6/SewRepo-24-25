namespace Chat.Api.Models;

public class UpdateSecuritySettings
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public bool EnableTwoFactor { get; set; }
}