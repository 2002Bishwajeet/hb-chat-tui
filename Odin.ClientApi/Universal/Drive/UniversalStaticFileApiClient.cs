using Odin.Core.Identity;
using Odin.Hosting.Controllers.Base.Cdn;
using Odin.Hosting.Controllers.OwnerToken.Cdn;
using Odin.Services.Optimization.Cdn;
using Refit;

namespace Odin.ClientApi.Universal.Drive;

public class UniversalStaticFileApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<StaticFilePublishResult>> Publish(PublishStaticFileRequest publishRequest)
    {
        var svc = factory.CreateRefitHttpClient<IUniversalStaticFileHttpClientApi>(identity);
        var response = await svc.Publish(publishRequest);
        return response;
    }

    public async Task<ApiResponse<HttpContent>> PublishPublicProfileImage(PublishPublicProfileImageRequest request)
    {
        var svc = factory.CreateRefitHttpClient<IUniversalStaticFileHttpClientApi>(identity);
        return await svc.PublishPublicProfileImage(request);
    }

    public async Task<ApiResponse<HttpContent>> PublishPublicProfileCard(PublishPublicProfileCardRequest request)
    {
        var svc = factory.CreateRefitHttpClient<IUniversalStaticFileHttpClientApi>(identity);
        return await svc.PublishPublicProfileCard(request);
    }

    public async Task<ApiResponse<HttpContent>> GetPublicProfileCard()
    {
        var svc = factory.CreateRefitHttpClient<IUniversalPublicStaticFileHttpClientApi>(identity);
        return await svc.GetPublicProfileCard();
    }

    public async Task<ApiResponse<HttpContent>> GetPublicProfileImage()
    {
        var svc = factory.CreateRefitHttpClient<IUniversalPublicStaticFileHttpClientApi>(identity);
        var response = await svc.GetPublicProfileImage();
        return response;
    }

    public async Task<ApiResponse<HttpContent>> GetStaticFile(string filename)
    {
        var svc = factory.CreateRefitHttpClient<IUniversalPublicStaticFileHttpClientApi>(identity);
        return await svc.GetStaticFile(filename);
    }
}