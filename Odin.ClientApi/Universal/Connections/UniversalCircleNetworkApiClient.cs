using Odin.Core;
using Odin.Core.Identity;
using Odin.Hosting.Controllers;
using Odin.Hosting.Controllers.Base.Membership.Connections;
using Odin.Services.Base;
using Odin.Services.Membership.Circles;
using Odin.Services.Membership.Connections;
using Odin.Services.Membership.Connections.Requests;
using Refit;

namespace Odin.ClientApi.Universal.Connections;

public class UniversalCircleNetworkApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<HttpContent>> CreateCircle(Guid id, string circleName, PermissionSetGrantRequest grant)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleDefinition>(identity);

        var request = new CreateCircleRequest()
        {
            Id = id,
            Name = circleName,
            Description = $"Description for {circleName}",
            DriveGrants = grant.Drives,
            Permissions = grant.PermissionSet
        };

        var createCircleResponse = await svc.CreateCircleDefinition(request);
        return createCircleResponse;
    }

    public async Task<ApiResponse<CircleDefinition>> GetCircleDefinition(GuidId circleId)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleDefinition>(identity);
        var response = await svc.GetCircleDefinition(circleId);
        return response;
    }

    public async Task<ApiResponse<IEnumerable<CircleDefinition>>> GetCircleDefinitions(bool includeSystemCircle)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleDefinition>(identity);

        var response = await svc.GetCircleDefinitions(includeSystemCircle);
        return response;
    }

    public async Task<ApiResponse<HttpContent>> UpdateCircleDefinition(CircleDefinition definition)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleDefinition>(identity);
        var response = await svc.UpdateCircleDefinition(definition);
        return response;
    }

    public async Task<ApiResponse<HttpContent>> GrantCircle(Guid circleId, OdinId odinId)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.AddCircle(new AddCircleMembershipRequest()
        {
            CircleId = circleId,
            OdinId = odinId
        });

        return apiResponse;
    }

    public async Task<ApiResponse<HttpContent>> RevokeCircle(Guid circleId, OdinId odinId)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.RevokeCircle(new RevokeCircleMembershipRequest()
        {
            CircleId = circleId,
            OdinId = odinId
        });

        return apiResponse;
    }

    public async Task<ApiResponse<IEnumerable<OdinId>>> GetCircleMembers(Guid circleId)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.GetCircleMembers(new GetCircleMembersRequest() { CircleId = circleId });
        return apiResponse;
    }

    public async Task<ApiResponse<RedactedIdentityConnectionRegistration>> GetConnectionInfo(OdinId recipient)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.GetConnectionInfo(new OdinIdRequest() { OdinId = recipient });
        return apiResponse;
    }

    public async Task<ApiResponse<HttpContent>> BlockConnection(OdinId odinId)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.Block(new OdinIdRequest() { OdinId = odinId });
        return apiResponse;
    }

    public async Task<ApiResponse<HttpContent>> UnblockConnection(OdinId odinId)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.Unblock(new OdinIdRequest() { OdinId = odinId });
        return apiResponse;
    }

    public async Task<ApiResponse<HttpContent>> DisconnectFrom(OdinId recipient)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var disconnectResponse = await svc.Disconnect(new OdinIdRequest() { OdinId = recipient });
        return disconnectResponse;
    }

    public async Task<ApiResponse<IcrVerificationResult>> VerifyConnection(OdinId recipient)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.VerifyConnection(new OdinIdRequest() { OdinId = recipient });
        return apiResponse;
    }

    public async Task<ApiResponse<IcrVerificationResult>> ConfirmConnection(OdinId recipient)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var apiResponse = await svc.ConfirmConnection(new OdinIdRequest() { OdinId = recipient });
        return apiResponse;
    }
}