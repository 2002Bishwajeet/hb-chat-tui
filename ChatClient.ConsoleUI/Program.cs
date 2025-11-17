using ChatClient.Core.Services;
using ChatClient.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace ChatClient.ConsoleUI;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<IConversationService, ConversationService>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IMessageService, MessageService>();

        var serviceProvider = services.BuildServiceProvider();

        Application.Init();
        var top = Application.Top;

        if (top != null)
        {
            // Create the main window
            var win = new Window()
            {
                Title = "Chat Client",
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            top.Add(win);
        }

        Application.Run();
    }
}
