using HttpClientFactoryLite;
using Odin.ClientApi.App;
using Odin.ClientApi.Factory;
using Odin.ClientApi.Owner.ApiClient;

namespace Odin.ClientApi;

public static class OdinApiHttpClientFactory
{
    private static readonly HttpClientFactory HttpClientFactory = new();

    static OdinApiHttpClientFactory()
    {
        HttpClientFactory.Register<AppApiClientFactory>(b =>
            b.ConfigurePrimaryHttpMessageHandler(() => new SharedSecretGetRequestHandler
            {
                UseCookies = false // DO NOT CHANGE!
            }));

        HttpClientFactory.Register<OwnerApiClient>(b =>
            b.ConfigurePrimaryHttpMessageHandler(() => new SharedSecretGetRequestHandler
            {
                UseCookies = false // DO NOT CHANGE!
            }));
    }
    
    
    public static HttpClient CreateHttpClient<T>()
    {
        return HttpClientFactory.CreateClient<T>();
    }
}