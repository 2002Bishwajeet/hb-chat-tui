namespace ChatClient.Core.Models;

public class Message
{
    public Guid Id { get; set; }
    public User Author { get; set; } = new();
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public MessageStatus Status { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
}
