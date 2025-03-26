using ChatApplication.Repositories;
using Database.Entities;

namespace ChatApplication.Services;

/*
public class MessageService(ARepository repository)
{
    private readonly ARepository _repository = repository;
    
    public async Task<List<Message>> GetAllMessagesAsync() => await _repository.GetAllMessagesAsync();

    public async Task<Message?> GetMessageByIdAsync(int id) => await _repository.GetMessageByIdAsync(id);
    public async Task AddMessageAsync(Message message)
    {
        message.Timestamp = DateTime.Now;
        await _repository.AddMessageAsync(message);
    }
}

*/