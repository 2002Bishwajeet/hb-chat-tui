using Refit;

namespace Odin.ClientApi.Universal.Drive
{
    /// <summary>
    /// The interface for storing files
    /// </summary>
    public interface IUniversalPublicStaticFileHttpClientApi
    {

        [Get("/cdn/{filename}")]
        Task<ApiResponse<HttpContent>> GetStaticFile(string filename);
        
        [Get("/pub/image")]
        Task<ApiResponse<HttpContent>> GetPublicProfileImage();
        
        [Get("/pub/profile")]
        Task<ApiResponse<HttpContent>> GetPublicProfileCard();
    }
}