namespace ChatClient.Core.Services;

public interface IAuthService
{
    Task<bool> AuthenticateAsync(string homebaseId);
    Task LogoutAsync();
}
