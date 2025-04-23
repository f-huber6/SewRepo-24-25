using System.Text.RegularExpressions;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Chat.Api.Controllers;

public class EmailService: IEmailSender
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration) => _configuration = configuration;

    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Smtp:Username"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlMessage
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_configuration["Smtp:Server"], int.Parse(_configuration["Smtp:Port"]!),
            SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}