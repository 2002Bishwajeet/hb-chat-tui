using ChatClient.Core.Models;

namespace ChatClient.Core.Services;

public interface IConversationService
{
    Task<IEnumerable<Conversation>> GetConversationsAsync();
}
