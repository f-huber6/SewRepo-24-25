using Database.Context;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Repositories;

public class MessageRepository(ApplicationContext context)
{
    private readonly ApplicationContext _context = context;

    public async Task<List<Message>> GetAllMessagesAsync() => await _context.Messages.ToListAsync();

    public async Task<Message?> GetMessageByIdAsync(int id) => await _context.Messages.FindAsync(id);

    public async Task AddMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }
}