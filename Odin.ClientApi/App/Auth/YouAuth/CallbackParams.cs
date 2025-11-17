#nullable enable

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Odin.Hosting.ApiExceptions.Client;
using Odin.Services.Authentication.YouAuth;

namespace Odin.ClientApi.App.Auth.YouAuth;

public class CallbackParams
{
    [BindProperty(Name = YouAuthDefaults.Identity, SupportsGet = true)]
    public string Identity { get; set; } = "";
        
    [BindProperty(Name = YouAuthDefaults.PublicKey, SupportsGet = true)]
    public string PublicKey { get; set; } = "";
        
    [BindProperty(Name = YouAuthDefaults.Salt, SupportsGet = true)]
    public string Salt { get; set; } = "";
        
    [BindProperty(Name = YouAuthDefaults.State, SupportsGet = true)]
    public string? State { get; set; } = "";
        
}

public class YouAuthTokenResponse
{
    public string? Base64SharedSecretCipher { get; set; }
    public string? Base64SharedSecretIv { get; set; }

    public string? Base64ClientAuthTokenCipher { get; set; }
    public string? Base64ClientAuthTokenIv { get; set; }
}


public class YouAuthTokenRequest
{
    public const string SecretDigestName = "secret_digest";
    [JsonPropertyName(SecretDigestName)]
    public string SecretDigest { get; set; } = "";

    //

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SecretDigest))
        {
            throw new BadRequestException($"{SecretDigestName} is required");
        }
    }
}