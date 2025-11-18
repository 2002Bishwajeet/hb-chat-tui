using System;
using ChatClient.Core.Services;
using Terminal.Gui;

namespace ChatClient.ConsoleUI.Views;

public class LoginView : Window
{
    private readonly IAuthService _authService;
    private readonly TextField _homebaseIdField;
    private readonly Button _loginButton;
    private readonly Label _errorLabel;

    public event Action LoginSuccessful;

    public LoginView(IAuthService authService)
    {
        _authService = authService;

        Title = "Login";
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();

        var asciiArt = new Label(@"
   _   _                           _   _                 _   _          _
  | | | |                         | | | |               | | | |        | |
  | |_| | ___  _ __ ___   ___     | |_| |__   ___     __| | | |__   ___| |_
  |  _  |/ _ \| '_ ` _ \ / _ \    | __| '_ \ / _ \   / _` | | '_ \ / _ \ __|
  | | | | (_) | | | | | |  __/    | |_| | | |  __/  | (_| | | | | |  __/ |_
  \_| |_/\___/|_| |_| |_|\___|     \__|_| |_|\___|   \__,_|_|_| |_|\___|\__|


")
        {
            X = Pos.Center(),
            Y = 2
        };
        Add(asciiArt);

        var homebaseIdLabel = new Label("Homebase ID:")
        {
            X = Pos.Center() - 15,
            Y = 12
        };
        _homebaseIdField = new TextField("")
        {
            X = Pos.Center(),
            Y = 12,
            Width = 30
        };
        Add(homebaseIdLabel, _homebaseIdField);

        _loginButton = new Button("Login")
        {
            X = Pos.Center() - 5,
            Y = 14
        };
        _loginButton.Clicked += OnLoginButtonClicked;
        Add(_loginButton);

        _errorLabel = new Label("")
        {
            X = Pos.Center(),
            Y = 16,
            TextColor = Application.Driver.MakeAttribute(Color.Red, Color.Black)
        };
        Add(_errorLabel);
    }

    private async void OnLoginButtonClicked()
    {
        _errorLabel.Text = "";
        var homebaseId = _homebaseIdField.Text.ToString();

        if (string.IsNullOrEmpty(homebaseId))
        {
            _errorLabel.Text = "Homebase ID cannot be empty.";
            return;
        }

        if (!await _authService.CheckIdentityAsync(homebaseId))
        {
            _errorLabel.Text = "Invalid Homebase ID.";
            return;
        }

        if (await _authService.AuthenticateAsync(homebaseId))
        {
            LoginSuccessful?.Invoke();
        }
        else
        {
            _errorLabel.Text = "Authentication failed.";
        }
    }
}
