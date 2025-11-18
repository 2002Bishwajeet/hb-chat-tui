namespace ChatClient.Core.Services;

public interface IConfigService
{
    Task SaveCredentialsAsync(string homebaseId, string authToken, string sharedSecret);
    Task<(string homebaseId, string authToken, string sharedSecret)> LoadCredentialsAsync();
    Task ClearCredentialsAsync();
}
