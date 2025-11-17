namespace ChatClient.Core.Services;

public interface IAuthService
{
    Task<bool> CheckIdentityAsync(string homebaseId);
    Task<string> PrepareAuthenticationRequestUrl(string homebaseId);
    Task<bool> AuthenticateAsync(string homebaseId);
    Task<bool> IsAuthenticatedAsync();
    Task LogoutAsync();
}
