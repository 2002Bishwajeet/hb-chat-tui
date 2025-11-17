using Refit;

namespace Odin.ClientApi.App.Auth
{
    public interface IRefitAppAuth
    {
        private const string RootPath = "/auth";

        [Get(RootPath + "/verifyToken")]
        Task<ApiResponse<HttpContent>> VerifyToken(CancellationToken cancellationToken = default);

        [Post(RootPath + "/logout")]
        Task<ApiResponse<HttpContent>> Logout();
    }
}