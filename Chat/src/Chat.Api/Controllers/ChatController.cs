using System.Security.Claims;
using Chat.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Database.Context;
using Chat.Database.Entities;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController: ControllerBase
{
    private readonly ApplicationContext _context;

    public ChatController(ApplicationContext context) => _context = context;

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessageModel messageModel)
    {
        var message = new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = messageModel.SenderId,
            ReceiverId = messageModel.ReceiverId,
            Text = messageModel.Text,
            SentAt = DateTime.Now,
            IsSeen = false,
            SeenAt = null
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        
        var result = new ChatMessageModel
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Text = message.Text,
            SentAt = message.SentAt,
            IsSeen = message.IsSeen,
            SeenAt = message.SeenAt
        };

        return Ok(result);
    }

    [HttpGet("history/{friendId}")]
    public async Task<IActionResult> GetChatHistory(string friendId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
            return Unauthorized("User is not logged in");
        
        var messages = await _context.ChatMessages
            .Include(s => s.ChatFiles)
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == friendId) ||
                (m.SenderId == friendId && m.ReceiverId == userId)
            )
            .OrderBy(s => s.SentAt)
            .ToListAsync();

        var chatHistory = messages.Select(m => new ChatMessageModel
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Text = m.Text,
            SentAt = m.SentAt,
            IsSeen = m.IsSeen,
            SeenAt = m.SeenAt,
            Files = m.ChatFiles?.Select(f => new ChatFileModel
            {
                FileName    = f.FileName ?? "",
                FileUrl     = f.FileUrl ?? "",
                ContentType = f.ContentType,
                FileSize    = f.FileSize,
                UploadedAt  = f.UploadedAt ?? DateTime.Now
            }).ToList()
        }).ToList();

        return Ok(chatHistory);
    }

    [HttpPost("mark-as-seen")]
    public async Task<IActionResult> MarkAsSeen([FromBody] string friendId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var messages = await _context.ChatMessages
            .Where(m => m.SenderId == friendId && m.ReceiverId == userId && !m.IsSeen)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsSeen = true;
            message.SeenAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("upload-file")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile? file, [FromForm] string messageId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Invalid file");

        var uploadId       = Guid.NewGuid().ToString();
        var uploadFileName = $"{uploadId}_{file.FileName}";
        var uploadPath     = Path.Combine("wwwroot", "uploads", uploadFileName);

        Directory.CreateDirectory(Path.GetDirectoryName(uploadPath)!);

        await using var stream = new FileStream(uploadPath, FileMode.Create);
        await file.CopyToAsync(stream);

        var chatFile = new ChatFile
        {
            FileName    = file.FileName,
            FileUrl     = $"/uploads/{uploadFileName}",
            ContentType = file.ContentType,
            FileSize    = file.Length,
            UploadedAt  = DateTime.Now,
            ChatMessageId = messageId
        };

        _context.ChatFiles.Add(chatFile);
        await _context.SaveChangesAsync();
        
        var result = new ChatFileModel
        {
            FileName = chatFile.FileName!,
            FileUrl = chatFile.FileUrl!,
            ContentType = chatFile.ContentType,
            FileSize = chatFile.FileSize,
            UploadedAt = chatFile.UploadedAt ?? DateTime.UtcNow
        };

        return Ok(result);
    }

    [HttpGet("unread-counts")]
    public async Task<IActionResult> GetUnreadCounts()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
            return Unauthorized("User is not logged in");

        var counts = await _context.ChatMessages
            .Where(s => s.ReceiverId == userId && !s.IsSeen)
            .GroupBy(m => m.SenderId)
            .Select(g => new UnreadCountModel
            {
                FriendId = g.Key,
                UnreadCount = g.Count()
            }).ToListAsync();

        return Ok(counts);
    }
}