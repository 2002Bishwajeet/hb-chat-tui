using ChatClient.Core.Models;
using ChatClient.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatClient.Infrastructure.Services;

public class MessageService : IMessageService
{
    public Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId)
    {
        return Task.FromResult(new List<Message>().AsEnumerable());
    }

    public Task SendMessageAsync(Message message)
    {
        return Task.CompletedTask;
    }
}
