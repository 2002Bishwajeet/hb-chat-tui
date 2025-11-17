using Odin.Core;
using Odin.Core.Identity;
using Odin.Services.DataSubscription.Follower;
using Odin.Services.Drives;
using Refit;

namespace Odin.ClientApi.Universal.Follower;

public class UniversalFollowerApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<HttpContent>> FollowIdentity(OdinId peerIdentity, FollowerNotificationType notificationType,
        List<TargetDrive>? channels = null)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalFollowerClient>(identity);
        var request = new FollowRequest()
        {
            OdinId = peerIdentity,
            NotificationType = notificationType,
            Channels = channels ?? []
        };

        var apiResponse = await svc.Follow(request);
        return apiResponse;
    }

    public async Task<ApiResponse<HttpContent>> UnfollowIdentity(OdinId peerIdentity)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalFollowerClient>(identity);
        var request = new UnfollowRequest()
        {
            OdinId = peerIdentity,
        };

        return await svc.Unfollow(request);
    }

    public async Task<ApiResponse<CursoredResult<string>>> GetIdentitiesIFollow(string cursor)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalFollowerClient>(identity);
        var apiResponse = await svc.GetIdentitiesIFollow(cursor);
        return apiResponse;
    }

    public async Task<ApiResponse<CursoredResult<string>>> GetIdentitiesFollowingMe(string cursor)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalFollowerClient>(identity);
        var apiResponse = await svc.GetIdentitiesFollowingMe(cursor);
        return apiResponse;
    }

    public async Task<ApiResponse<FollowerDefinition>> GetFollower(OdinId peerIdentity)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalFollowerClient>(identity);
        var apiResponse = await svc.GetFollower(peerIdentity);
        return apiResponse;
    }

    public async Task<ApiResponse<FollowerDefinition>> GetIdentityIFollow(OdinId peerIdentity)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalFollowerClient>(identity);
        var apiResponse = await svc.GetIdentityIFollow(peerIdentity);
        return apiResponse;
    }

    public async Task<ApiResponse<HttpContent>> SynchronizeFeed(OdinId peerIdentity)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalFollowerClient>(identity);
        var apiResponse = await svc.SynchronizeFeedHistory(new SynchronizeFeedHistoryRequest()
        {
            OdinId = peerIdentity
        });
        return apiResponse;
    }
}