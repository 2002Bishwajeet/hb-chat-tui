using ChatClient.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatClient.Infrastructure.Data;

public class ChatContext : DbContext
{
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Attachment> Attachments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=chat.db");
    }
}
