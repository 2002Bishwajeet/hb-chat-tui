using ChatClient.Core.Models;

namespace ChatClient.Core.Services;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId);
    Task SendMessageAsync(Message message);
}
