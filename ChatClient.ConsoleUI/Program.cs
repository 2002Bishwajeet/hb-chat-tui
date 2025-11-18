using ChatClient.ConsoleUI.Views;
using ChatClient.Core.Services;
using ChatClient.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace ChatClient.ConsoleUI;

class Program
{
    static async Task Main(string[] args)
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

        var authService = serviceProvider.GetRequiredService<IAuthService>();
        if (await authService.IsAuthenticatedAsync())
        {
            ShowMainWindow(top);
        }
        else
        {
            var loginView = new LoginView(authService);
            loginView.LoginSuccessful += () =>
            {
                top.Remove(loginView);
                ShowMainWindow(top);
            };
            top.Add(loginView);
        }

        Application.Run();
    }

    private static void ShowMainWindow(Toplevel top)
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
}
