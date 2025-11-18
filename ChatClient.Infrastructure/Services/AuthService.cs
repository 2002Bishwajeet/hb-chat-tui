using ChatClient.Core.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Odin.ClientApi;
using Odin.ClientApi.App;
using Odin.ClientApi.App.Auth;
using Odin.ClientApi.App.Auth.YouAuth;
using Odin.Core;
using Odin.Core.Cryptography.Crypto;
using Odin.Core.Cryptography.Data;
using Odin.Core.Identity;
using Odin.Core.Serialization;
using Odin.Services.Authentication.YouAuth;
using Odin.Services.Authorization.ExchangeGrants;
using Odin.Services.Drives;
using Refit;

namespace ChatClient.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient = new();
    private readonly IConfigService _configService;
    private HttpListener? _httpListener;

    private readonly Dictionary<string, (EccFullKeyData keyPair, string homebaseId)> _authCache = new();

    private OdinApiAppClient? _apiClient;
    private OdinId? _odinId;

    private const string AppId = "9a8b7c6d-5e4f-3a2b-1c0d-9e8f7a6b5c4d";
    private const string AppName = "Homebase - Community & Chat TUI";

    private const string ConfirmedConnectionsCircleId = "bb2683fa402aff866e771a6495765a15";
    private const string AutoConnectionsCircleId = "9e22b42952f74d2580e11250b651d343";

    public AuthService(IConfigService configService)
    {
        _configService = configService;
    }

    public async Task<bool> CheckIdentityAsync(string homebaseId)
    {
        if (string.IsNullOrEmpty(homebaseId))
        {
            return false;
        }

        const string domainRegex = @"^(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z0-9]{2,25}(?::\d{1,5})?$";
        if (!Regex.IsMatch(homebaseId, domainRegex, RegexOptions.IgnoreCase))
        {
            return false;
        }

        try
        {
            var url = $"https://{homebaseId}/api/guest/v1/auth/ident";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var validation = JsonSerializer.Deserialize<IdentValidationResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return validation?.OdinId?.Equals(homebaseId, System.StringComparison.OrdinalIgnoreCase) ?? false;
        }
        catch
        {
            return false;
        }
    }

    public Task<string> PrepareAuthenticationRequestUrl(string homebaseId)
    {
        var privateKey = new byte[48];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(privateKey);
        }
        var keyPair = new EccFullKeyData(privateKey.ToSensitiveByteArray(), EccKeySize.P384, 1);
        var publicKey = keyPair.PublicKeyJwkBase64Url();
        var state = Guid.NewGuid().ToString("n");
        _authCache[state] = (keyPair, homebaseId);

        var drives = new List<DriveParam>
        {
            new() { DriveAlias = "9ff813aff2d61e2f9b9db189e72d1a11", DriveType = "66ea8355ae4155c39b5a719166b510e3", Name = "Chat Drive", Description = "", DrivePermission = (int)(DrivePermission.Read | DrivePermission.Write | DrivePermission.React) },
            new() { DriveAlias = "8f12d8c4933813d378488d91ed23b64c", DriveType = "597241530e3ef24b28b9a75ec3a5c45c", Name = "", Description = "", DrivePermission = (int)DrivePermission.Read },
            new() { DriveAlias = "2612429d1c3f037282b8d42fb2cc0499", DriveType = "70e92f0f94d05f5c7dcd36466094f3a5", Name = "", Description = "", DrivePermission = (int)(DrivePermission.Read | DrivePermission.Write) },
            new() { DriveAlias = "3e5de26f-8fa3-43c1-975a-d0dd2aa8564c", DriveType = "93a6e08d-14d9-479e-8d99-bae4e5348a16", Name = "Community Drive", Description = "", DrivePermission = (int)(DrivePermission.Read | DrivePermission.Write) }
        };

        var permissions = new List<string>
        {
            "ReadConnections",
            "ReadConnectionRequests",
            "ReadCircleMembers",
            "SendDataToOtherIdentitiesOnMyBehalf",
            "ReceiveDataFromOtherIdentitiesOnMyBehalf",
            "SendPushNotifications",
            "SendIntroductions"
        };

        var circleDrives = new List<DriveParam>
        {
            new()
            {

                DriveAlias = "9ff813aff2d61e2f9b9db189e72d1a11",
                DriveType = "66ea8355ae4155c39b5a719166b510e3",
                Name = "Chat Drive",
                Description = "",
                DrivePermission = (int)(DrivePermission.Write | DrivePermission.React),
                AllowAnonymousReads = null,
                AllowSubscriptions = null
            }
        };

        var appParameters = new AppAuthorizationParams
        {
            AppId = AppId,
            AppName = AppName,
            ClientFriendlyName = "Chat Client TUI",
            PermissionKeysCsv = string.Join(",", permissions),
            DriveAccessJson = OdinSystemSerializer.Serialize(drives),
            CirclePermissionKeysCsv = "[]",
            CircleDriveAccessJson = OdinSystemSerializer.Serialize(circleDrives),
            CircleIdsCsv = $"{ConfirmedConnectionsCircleId},{AutoConnectionsCircleId}",
            ReturnBehavior = "redirect",
            AppCorsOrigin = null
        };

        var authRequest = new YouAuthAuthorizeRequest
        {
            ClientId = AppId,
            ClientType = Odin.Services.Authentication.YouAuth.ClientType.app,
            ClientInfo = "Chat Client TUI",
            PublicKey = publicKey,
            RedirectUri = "http://localhost:8080/auth/callback",
            PermissionRequest = OdinSystemSerializer.Serialize(appParameters),
            State = state
        };

        return Task.FromResult($"https://{homebaseId}/api/owner/v1/youauth/authorize?{authRequest.ToQueryString()}");
    }

    public async Task<bool> AuthenticateAsync(string homebaseId)
    {
        var url = await PrepareAuthenticationRequestUrl(homebaseId);
        OpenBrowser(url);

        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add("http://localhost:8080/auth/callback/");
        _httpListener.Start();

        var context = await _httpListener.GetContextAsync();
        var request = context.Request;

        var response = context.Response;
        var buffer = System.Text.Encoding.UTF8.GetBytes("<html><body><h1>Success!</h1><p>You can close this window now.</p></body></html>");
        response.ContentLength64 = buffer.Length;
        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
        _httpListener.Stop();

        var state = request.QueryString.Get("state");
        if (string.IsNullOrEmpty(state) || !_authCache.TryGetValue(state, out var cacheItem))
        {
            return false;
        }

        var callbackParams = new CallbackParams
        {
            Identity = request.QueryString.Get("identity"),
            PublicKey = request.QueryString.Get("public_key"),
            Salt = request.QueryString.Get("salt"),
            State = state
        };

        return await FinalizeAppAuthRequest(callbackParams);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var (homebaseId, authToken, sharedSecret) = await _configService.LoadCredentialsAsync();
        if (string.IsNullOrEmpty(homebaseId) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(sharedSecret))
        {
            return false;
        }

        var token = ClientAuthenticationToken.FromPortableBytes64(authToken);
        var sharedSecretBytes = Convert.FromBase64String(sharedSecret);
        var cat = new ClientAccessToken
        {
            Id = token.Id,
            AccessTokenHalfKey = token.AccessTokenHalfKey,
            ClientTokenType = token.ClientTokenType,
            SharedSecret = sharedSecretBytes.ToSensitiveByteArray()
        };

        var odinId = new OdinId(homebaseId);
        var client = new OdinApiAppClient(odinId, cat);

        if (!await client.VerifyToken())
        {
            await _configService.ClearCredentialsAsync();
            return false;
        }

        _apiClient = client;
        _odinId = odinId;
        return true;
    }

    public async Task<bool> FinalizeAppAuthRequest(CallbackParams callbackParams)
    {
        if (string.IsNullOrEmpty(callbackParams.State) || !_authCache.TryGetValue(callbackParams.State, out var cacheItem))
        {
            return false;
        }

        var localKey = cacheItem.keyPair;
        var remotePublicKey = EccPublicKeyData.FromJwkBase64UrlPublicKey(callbackParams.PublicKey);
        var exchangeSecret = localKey.GetEcdhSharedSecret(localKey.FullKey.ToSensitiveByteArray(), remotePublicKey, Convert.FromBase64String(callbackParams.Salt));

        var exchangeSecretDigest = SHA256.Create().ComputeHash(exchangeSecret.GetKey()).ToBase64();
        var odinId = new OdinId(callbackParams.Identity);
        var tokenResponse = await ExchangeDigestForToken(odinId, exchangeSecretDigest, CancellationToken.None);

        if (null == tokenResponse)
        {
            return false;
        }

        var clientAuthTokenCipher = Convert.FromBase64String(tokenResponse.Base64ClientAuthTokenCipher!);
        var clientAuthTokenIv = Convert.FromBase64String(tokenResponse.Base64ClientAuthTokenIv!);
        var clientAuthTokenBytes = AesCbc.Decrypt(clientAuthTokenCipher, exchangeSecret, clientAuthTokenIv);
        var authenticationToken = ClientAuthenticationToken.FromPortableBytes(clientAuthTokenBytes);

        var sharedSecretCipher = Convert.FromBase64String(tokenResponse.Base64SharedSecretCipher!);
        var sharedSecretIv = Convert.FromBase64String(tokenResponse.Base64SharedSecretIv!);
        var sharedSecret = AesCbc.Decrypt(sharedSecretCipher, exchangeSecret, sharedSecretIv);

        var clientAccessToken = new ClientAccessToken
        {
            Id = authenticationToken.Id,
            AccessTokenHalfKey = authenticationToken.AccessTokenHalfKey,
            ClientTokenType = authenticationToken.ClientTokenType,
            SharedSecret = sharedSecret.ToSensitiveByteArray()
        };

        await _configService.SaveCredentialsAsync(odinId.ToString(), clientAccessToken.ToPortableBytes64(), Convert.ToBase64String(sharedSecret));

        return true;
    }

    public async Task LogoutAsync()
    {
        _apiClient = null;
        _odinId = null;
        await _configService.ClearCredentialsAsync();
    }

    private async Task<YouAuthTokenResponse> ExchangeDigestForToken(OdinId odinId, string digest, CancellationToken cancellationToken)
    {
        var tokenRequest = new YouAuthTokenRequest
        {
            SecretDigest = digest
        };

        var svc = CreateRefitService<IYouAuthRefitClient>(odinId);
        var response = await svc.ExchangeCodeForToken(tokenRequest, cancellationToken);

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return response.Content;
        }

        return null;
    }

    private T CreateRefitService<T>(OdinId odinId)
    {
        var client = new HttpClient { BaseAddress = new Uri($"https://{odinId}") };
        return RestService.For<T>(client);
    }

    private void OpenBrowser(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    private class IdentValidationResponse
    {
        public string OdinId { get; set; }
    }
}
