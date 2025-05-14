using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chat.Api.Models;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Chat.Database.Entities;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class IdentityController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityController> _logger;
    private readonly IEmailSender _emailService;
    
    public IdentityController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        ILogger<IdentityController> logger,
        IEmailSender emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.UserName,
            NormalizedUserName = model.UserName.ToUpper(),
            Email = model.Email,
            NormalizedEmail = model.UserName.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = model.Password,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberConfirmed = true, //ÄNDERN
            TwoFactorEnabled = false,
            LockoutEnabled = false, //ÄNDERN
            LockoutEnd = null, //ÄNDERN
            AccessFailedCount = 0 //ÄNDERN
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            _logger.LogError($"Error creating user: {string.Join(",", result.Errors)}");
            return BadRequest("User registration failed");
        }

        //using var channel = GrpcChannel.ForAddress("https://localhost:7034");
        //var client = new UserService.UserServiceClient(channel);
        //var reply = await client.AddUserAsync(new UserRequest()
        //{
        //    UserId = user.Id
        //});
        
        /*
        if (String.Equals(reply.Result, "Success", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new
            {
                Message = "User successfully registered, check your email"
            });   
        }
        
        _logger.LogError($"Error registering user: {string.Join(",", result.Errors)}");
        return BadRequest("User registration failed");
        */
        
        return Ok(new
        {
            Message = "User successfully registered, check your email"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(s => s.UserName == model.UserName);

        if (user == null)
        {
            _logger.LogWarning("User not found");
            return Unauthorized("User not found");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password!, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning($"Invalid password for User {model.UserName}");
            user.AccessFailedCount++;
            await _userManager.UpdateAsync(user);
            return Unauthorized("Invalid password");
        }

        if (!user.EmailConfirmed)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action("ConfirmEmail", "Identity", new
            {
                userId = user.Id,
                token = token
            }, Request.Scheme);

            var emailBody = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>";
            await _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody);

            return Unauthorized(new
            {
                Message = "EmailNotConfirmed",
                UserId = user.Id,
                Email = user.Email
            });
        }

        if (user.TwoFactorEnabled)
        {
            var code = new Random().Next(100000, 999999).ToString();
            user.TwoFactorCode = code;
            user.TwoFactorCodeExpiration = DateTime.UtcNow.AddMinutes(5);
            await _userManager.UpdateAsync(user);

            var emailBody = $"Your two factor code is: {code}";
            await _emailService.SendEmailAsync(user.Email!, "Two factor code", emailBody);

            return Unauthorized(new
            {
                Message = "TwoFactorRequired",
                UserId = user.Id
            });
        }

        var tokenString = GenerateJwtToken(user);
        return Ok(
            new
            {
                Token = tokenString
            });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest("User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return BadRequest("Invalid token");

        return Ok("Email confirmed");
    }

    [HttpPost("send-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User not found");

        if (user.EmailConfirmed) return BadRequest("Already confirmed");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = Url.Action("ConfirmEmail", "Identity", new
        {
            userId = user.Id,
            token = token
        }, Request.Scheme);

        var emailBody = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>";
        await _emailService.SendEmailAsync(user.Email!, "Confirm email again", emailBody);

        return Ok("Email confirmation resent");
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFactorModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
            return BadRequest("User not found");

        if (user.TwoFactorCode != model.TwoFactorCode || user.TwoFactorCodeExpiration < DateTime.UtcNow)
            return BadRequest("Invalid or expired code");

        user.TwoFactorCode = null;
        user.TwoFactorCodeExpiration = DateTime.MinValue;
        await _userManager.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        return Ok(new
        {
            Token = token
        });
    }

    [Authorize]
    [HttpGet("user-info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        _logger.LogWarning("UserId: {userId}", userId);
        if (user == null)
            return NotFound("User not found");

        return Ok(new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.UserName,
            user.TwoFactorEnabled
        });
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
            return NotFound("User not found");

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.UserName = model.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest("Update failed");

        return Ok("User info updated");
    }

    [Authorize]
    [HttpPut("update-security")]
    public async Task<IActionResult> UpdateSecuritySettings([FromBody] UpdateSecuritySettings securitySettings)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
            return NotFound("User not found");

        var changePasswordResult =
            await _userManager.ChangePasswordAsync(user, securitySettings.CurrentPassword,
                securitySettings.NewPassword);

        if (!changePasswordResult.Succeeded)
            return BadRequest("Password change failed");

        user.TwoFactorEnabled = securitySettings.EnableTwoFactor;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest("Update failed");

        return Ok("User security settings updated");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return NotFound("User not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ForgotPassword", "Identity", new
        {
            userId = user.Id,
            token = token
        }, Request.Scheme);

        var emailBody = $"Reset your password by clicking <a href='{resetLink}'>here</a>";
        await _emailService.SendEmailAsync(user.Email!, "Reset password", emailBody);

        return Ok("Password reset link sent");
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}