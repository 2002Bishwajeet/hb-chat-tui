using System.Threading;
using System.Threading.Tasks;
using Odin.ClientApi.App.Auth;
using Odin.Services.Authentication.YouAuth;
using Refit;

namespace ChatClient.Infrastructure.Services;

public interface IYouAuthRefitClient
{
    [Post("/api/owner/v1/youauth/token")]
    Task<ApiResponse<YouAuthTokenResponse>> ExchangeCodeForToken(
        [Body] YouAuthTokenRequest request,
        CancellationToken cancellationToken);
}
