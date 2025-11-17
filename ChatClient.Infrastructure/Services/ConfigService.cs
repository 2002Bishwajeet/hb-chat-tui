using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatClient.Core.Services;

namespace ChatClient.Infrastructure.Services;

public class ConfigService : IConfigService
{
    private readonly string _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".homebase-chat-tui.json");

    public async Task SaveCredentialsAsync(string homebaseId, string authToken, string sharedSecret)
    {
        var data = new Credentials { HomebaseId = homebaseId, AuthToken = authToken, SharedSecret = sharedSecret };
        var json = JsonSerializer.Serialize(data);
        var encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), null, DataProtectionScope.CurrentUser);
        await File.WriteAllBytesAsync(_configFilePath, encryptedData);
    }

    public async Task<(string homebaseId, string authToken, string sharedSecret)> LoadCredentialsAsync()
    {
        if (!File.Exists(_configFilePath))
        {
            return (null, null, null);
        }

        var encryptedData = await File.ReadAllBytesAsync(_configFilePath);
        var data = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
        var json = Encoding.UTF8.GetString(data);
        var credentials = JsonSerializer.Deserialize<Credentials>(json);
        return (credentials.HomebaseId, credentials.AuthToken, credentials.SharedSecret);
    }

    public Task ClearCredentialsAsync()
    {
        if (File.Exists(_configFilePath))
        {
            File.Delete(_configFilePath);
        }
        return Task.CompletedTask;
    }

    private class Credentials
    {
        public string HomebaseId { get; set; }
        public string AuthToken { get; set; }
        public string SharedSecret { get; set; }
    }
}
