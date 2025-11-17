using Odin.Core.Identity;
using Refit;

namespace Odin.ClientApi.App.Auth;

public class AppAuthApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<HttpContent>> VerifyToken(CancellationToken cancellationToken = default)
    {
        var svc = factory.CreateRefitHttpClient<IRefitAppAuth>(identity);
        return await svc.VerifyToken(cancellationToken);
    }

    public async Task<ApiResponse<HttpContent>> Logout()
    {
        var svc = factory.CreateRefitHttpClient<IRefitAppAuth>(identity);
        return await svc.Logout();
    }
}