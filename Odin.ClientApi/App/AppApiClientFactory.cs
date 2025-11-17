using Odin.Core;
using Odin.Core.Identity;
using Odin.Core.Storage;
using Odin.Hosting.Authentication.YouAuth;
using Odin.Hosting.Controllers.ClientToken.App;
using Odin.Services.Authorization.ExchangeGrants;
using Odin.Services.Base;

namespace Odin.ClientApi.App;

public class AppApiClientFactory(ClientAuthenticationToken token, byte[] secret, int? port = null) : IApiClientFactory
{
    public int Port { get; } = port.GetValueOrDefault(443);
    public SensitiveByteArray SharedSecret { get; } = secret.ToSensitiveByteArray();

    public HttpClient CreateHttpClient(OdinId identity, FileSystemType fileSystemType = FileSystemType.Standard)
    {
        var client = OdinApiHttpClientFactory.CreateHttpClient<AppApiClientFactory>();

        //
        // SEB:NOTE below is a hack to make SharedSecretGetRequestHandler work without instance data.
        // DO NOT do this in production code!
        //
        {
            var cookieValue = $"{YouAuthConstants.AppCookieName}={token}";
            client.DefaultRequestHeaders.Add("Cookie", cookieValue);
            client.DefaultRequestHeaders.Add("X-HACK-COOKIE", cookieValue);
            client.DefaultRequestHeaders.Add("X-HACK-SHARED-SECRET", Convert.ToBase64String(secret));
        }

        client.DefaultRequestHeaders.Add(OdinHeaderNames.FileSystemTypeHeader, Enum.GetName(fileSystemType));
        client.Timeout = TimeSpan.FromMinutes(15);

        var suffix = AppApiPathConstants.BasePathV1;
        client.BaseAddress = new Uri($"https://{identity}:{this.Port}{suffix}");
        return client;
    }
}