namespace ChatClient.Core.Models;

public class Conversation
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<User> Participants { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}
