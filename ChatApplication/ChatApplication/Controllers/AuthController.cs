using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApplication.Models;
using Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ChatApplication.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new User
        {
            UserName = model.UserName,
            NormalizedUserName = model.UserName?.ToUpper(),
            Email = model.Email,
            NormalizedEmail = model.Email?.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = model.Password,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            LockoutEnd = null,
            AccessFailedCount = 0
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (result.Succeeded)
        {
            _logger.LogInformation($"User {model.Email} created a new account with password.");
            return Ok(new {Message = "User registered successfully"});
        }
        
        return BadRequest("User registration failed");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            _logger.LogWarning("User {Email} not found", model.Email);
            return Unauthorized("Invalid username or password");   
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password!, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Invalid password for user {Email}", model.Email);
            user.AccessFailedCount++;
            await _userManager.UpdateAsync(user);

            if (user.AccessFailedCount >= 3)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.Now.AddMinutes(5);
                user.AccessFailedCount = 0;
                await _userManager.UpdateAsync(user);
                _logger.LogWarning("User {Email} locked out", model.Email);
                return Unauthorized("User locked out because of too many failed login attempts");
            }
                
            
            return Unauthorized("Invalid username or password");
        }

        var token = GenerateJwtToken(user);
        _logger.LogInformation("Current token: {Token}", token);
        
        _logger.LogInformation("User {Email} logged in", model.Email);
        return Ok(new
        {
            Token = token
        });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}