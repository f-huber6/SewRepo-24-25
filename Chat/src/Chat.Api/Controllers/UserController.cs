using System.Security.Claims;
using Chat.Api.Hub;
using Chat.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Chat.Database.Context;
using Chat.Database.Entities;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;

    public UserController(UserManager<User> userManager,
        ApplicationContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    [HttpPost("add-friend/{friendId}")]
    public async Task<IActionResult> AddFriend(string friendId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == friendId)
            return BadRequest("You cannot add yourself as a friend.");

        var friendUser = await _userManager.FindByIdAsync(friendId);
        if (friendUser == null)
            return NotFound("Friend not found.");

        var exists = await _context.FriendShips
            .AnyAsync(f => f.UserId == userId && f.FriendId == friendId);

        if (exists)
            return BadRequest("Friend already added.");

        _context.FriendShips.Add(new FriendShip
        {
            UserId = userId!,
            FriendId = friendId
        });

        await _context.SaveChangesAsync();
        
        var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<FriendHub>>();
        await hubContext.Clients.User(friendId).SendAsync("ReceiveFriendRequest");

        return Ok("Friend added successfully.");
    }
    
    [HttpGet("get-all-friends")]
    public async Task<IActionResult> GetAllFriends([FromQuery] string? search = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var query = _context.FriendShips
            .Include(f => f.Friend)
            .Where(f => f.UserId == userId);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(f => f.Friend!.UserName!.Contains(search));

        var friends = await query
            .Select(f => new UserModel
            {
                Id = f.Friend!.Id,
                FirstName = f.Friend.FirstName,
                LastName = f.Friend.LastName,
                UserName = f.Friend.UserName!
            })
            .ToListAsync();

        return Ok(friends);
    }
    
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var users = await _userManager.Users.Where(s => s.Id != userId).ToListAsync();

        if (!users.Any())
            return NotFound("No users found");

        var userModels = users.Select(user => new UserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName!
        }).ToList();

        return Ok(userModels);
    }
    
    [HttpGet("pending-requests")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) 
            return Unauthorized();

        var incomingRequestIds = await _context.FriendShips
            .Where(f => f.FriendId == userId)
            .Select(f => f.UserId)
            .ToListAsync();

        var existingFriendIds = await _context.FriendShips
            .Where(f => f.UserId == userId)
            .Select(f => f.FriendId)
            .ToListAsync();

        var pendingIds = incomingRequestIds.Except(existingFriendIds).ToList();

        var pendingUsers = await _context.Users
            .Where(u => pendingIds.Contains(u.Id))
            .Select(u => new UserModel
            {
                Id        = u.Id,
                FirstName = u.FirstName,
                LastName  = u.LastName,
                UserName  = u.UserName!
            })
            .ToListAsync();

        return Ok(pendingUsers);
    }
    
    [HttpDelete("remove-friend/{friendId}")]
    public async Task<IActionResult> RemoveFriend(string friendId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var friendships = await _context.FriendShips
            .Where(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId))
            .ToListAsync();

        _context.FriendShips.RemoveRange(friendships);
        
        var chatMessages = await _context.ChatMessages
            .Include(m => m.ChatFiles)
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == friendId) ||
                (m.SenderId == friendId && m.ReceiverId == userId))
            .ToListAsync();

        _context.ChatFiles.RemoveRange(chatMessages.SelectMany(m => m.ChatFiles!));
        _context.ChatMessages.RemoveRange(chatMessages);

        await _context.SaveChangesAsync();

        var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<ChatHub>>();
        await hubContext.Clients.User(friendId).SendAsync("FriendRemoved", userId);
        await hubContext.Clients.User(userId).SendAsync("FriendRemoved", friendId);

        return Ok();
    }
}