using Odin.Core;
using Odin.Core.Cryptography.Crypto;
using Odin.Core.Cryptography.Data;
using Odin.Core.Fluff;
using Odin.Core.Identity;
using Odin.Hosting.Controllers.OwnerToken.AppManagement;
using Odin.Services.Authorization.Apps;
using Odin.Services.Authorization.ExchangeGrants;
using Odin.Services.Base;
using Refit;

namespace Odin.ClientApi.Owner.ApiClient.AppManagement;

public class AppManagementApiClient(OdinId identity, IApiClientFactory factory)
{
    private IRefitAppRegistration CreateRefitClient()
    {
        var svc = factory.CreateRefitHttpClient<IRefitAppRegistration>(identity);
        return svc;
    }

    public async Task<(ClientAuthenticationToken clientAuthToken, byte[] sharedSecret)> RegisterAppClient(Guid appId)
    {
        var rsa = new RsaFullKeyData(RsaKeyListManagement.zeroSensitiveKey, 1); // TODO

        var svc = this.CreateRefitClient();
        var request = new AppClientRegistrationRequest()
        {
            AppId = appId,
            ClientPublicKey64 = Convert.ToBase64String(rsa.publicKey),
            ClientFriendlyName = "Some phone"
        };

        var regResponse = await svc.RegisterAppOnClient(request);

        var reply = regResponse.Content!;
        var decryptedData = rsa.Decrypt(RsaKeyListManagement.zeroSensitiveKey, reply.Data); // TODO

        var cat = ClientAccessToken.FromPortableBytes(decryptedData);
        return (cat.ToAuthenticationToken(), cat.SharedSecret.GetKey());
    }

    public async Task<ApiResponse<NoResultResponse>> RevokeApp(Guid appId)
    {
        var svc = this.CreateRefitClient();
        return await svc.RevokeApp(new GetAppRequest() { AppId = appId });
    }

    public async Task<ApiResponse<HttpContent>> UpdateAppAuthorizedCircles(Guid appId, List<Guid> authorizedCircles,
        PermissionSetGrantRequest grant)
    {
        var svc = this.CreateRefitClient();

        return await svc.UpdateAuthorizedCircles(new UpdateAuthorizedCirclesRequest()
        {
            AppId = appId,
            AuthorizedCircles = authorizedCircles,
            CircleMemberPermissionGrant = grant
        });
    }

    public async Task<ApiResponse<HttpContent>> UpdateAppPermissions(Guid appId, PermissionSetGrantRequest grant)
    {
        var svc = this.CreateRefitClient();
        return await svc.UpdateAppPermissions(new UpdateAppPermissionsRequest()
        {
            AppId = appId,
            Drives = grant.Drives,
            PermissionSet = grant.PermissionSet
        });
    }

    public async Task<ClientAccessToken> RegisterAppAndClient(Guid appId,
        PermissionSetGrantRequest appPermissions,
        List<Guid>? authorizedCircles = null,
        PermissionSetGrantRequest? circleMemberGrantRequest = null)
    {
        var appRegResponse = await this.RegisterApp(appId, appPermissions, authorizedCircles, circleMemberGrantRequest);
        if (!appRegResponse.IsSuccessStatusCode)
        {
            throw new Exception("Failed to register app");
        }

        var appClient = await this.RegisterAppClient(appId);
        return new ClientAccessToken
        {
            Id = appClient.clientAuthToken.Id,
            AccessTokenHalfKey = appClient.clientAuthToken.AccessTokenHalfKey,
            ClientTokenType = appClient.clientAuthToken.ClientTokenType,
            SharedSecret = appClient.sharedSecret.ToSensitiveByteArray()
        };
    }

    /// <summary>
    /// Creates an app, device, and logs in returning an contextual information needed to run unit tests.
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<RedactedAppRegistration>> RegisterApp(
        Guid appId,
        PermissionSetGrantRequest appPermissions,
        List<Guid>? authorizedCircles = null,
        PermissionSetGrantRequest? circleMemberGrantRequest = null)
    {
        var svc = this.CreateRefitClient();
        var request = new AppRegistrationRequest
        {
            Name = $"Test_{appId}",
            AppId = appId,
            PermissionSet = appPermissions.PermissionSet,
            Drives = appPermissions.Drives?.ToList(),
            AuthorizedCircles = authorizedCircles,
            CircleMemberPermissionGrant = circleMemberGrantRequest
        };

        var response = await svc.RegisterApp(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to register app with status code {response.StatusCode}");
        }

        var updatedAppResponse = await svc.GetRegisteredApp(new GetAppRequest() { AppId = appId });

        return updatedAppResponse;
    }
    
    public async Task<ApiResponse<RedactedAppRegistration>> RegisterApp(AppRegistrationRequest request)
    {
        var svc = this.CreateRefitClient();
        var response = await svc.RegisterApp(request);

        return response;
    }

    public async Task<ApiResponse<RedactedAppRegistration>> GetAppRegistration(Guid appId)
    {
        var svc = this.CreateRefitClient();
        var appResponse = await svc.GetRegisteredApp(new GetAppRequest() { AppId = appId });
        return appResponse;
    }

    public async Task<ApiResponse<List<RegisteredAppClientResponse>>> GetRegisteredClients(GuidId appId)
    {
        var svc = this.CreateRefitClient();
        var appResponse = await svc.GetRegisteredClients(new GetAppRequest() { AppId = appId });
        return appResponse;
    }
    
    public async Task<ApiResponse<NoResultResponse>> DeleteApp(GuidId appId)
    {
        var svc = this.CreateRefitClient();
        var appResponse = await svc.DeleteApp(new GetAppRequest() { AppId = appId });
        return appResponse;
    }
    
}