using ChatApplication.Repositories;
using Database.Entities;

namespace ChatApplication.Services;

public class MessageService(MessageRepository repository)
{
    private readonly MessageRepository _repository = repository;
    
    public async Task<List<Message>> GetAllMessagesAsync() => await _repository.GetAllMessagesAsync();

    public async Task AddMessageAsync(Message message)
    {
        message.Timestamp = DateTime.Now;
        await _repository.AddMessageAsync(message);
    }
}