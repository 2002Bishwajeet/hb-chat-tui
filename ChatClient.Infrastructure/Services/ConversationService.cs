using ChatClient.Core.Models;
using ChatClient.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatClient.Infrastructure.Services;

public class ConversationService : IConversationService
{
    public Task<IEnumerable<Conversation>> GetConversationsAsync()
    {
        return Task.FromResult(new List<Conversation>().AsEnumerable());
    }
}
