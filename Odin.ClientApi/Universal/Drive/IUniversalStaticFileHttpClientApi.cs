using Odin.Hosting.Controllers.Base.Cdn;
using Odin.Hosting.Controllers.OwnerToken.Cdn;
using Odin.Services.Optimization.Cdn;
using Refit;

namespace Odin.ClientApi.Universal.Drive
{
    /// <summary>
    /// The interface for storing files
    /// </summary>
    public interface IUniversalStaticFileHttpClientApi
    {
        private const string RootEndpoint = "/optimization/cdn";
        
        [Post(RootEndpoint + "/publish")]
        Task<ApiResponse<StaticFilePublishResult>> Publish([Body] PublishStaticFileRequest request);
        
        [Post(RootEndpoint + "/profileimage")]
        Task<ApiResponse<HttpContent>> PublishPublicProfileImage([Body] PublishPublicProfileImageRequest request);
        
        [Post(RootEndpoint + "/profilecard")]
        Task<ApiResponse<HttpContent>> PublishPublicProfileCard([Body] PublishPublicProfileCardRequest request);

    }
}