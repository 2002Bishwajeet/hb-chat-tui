using Odin.ClientApi.Factory;
using Odin.Core;
using Odin.Core.Identity;
using Odin.Core.Storage;
using Refit;

namespace Odin.ClientApi;

/// <summary>
/// Handles the creation of the Http Client
/// </summary>
public interface IApiClientFactory
{
    int Port { get; }

    SensitiveByteArray SharedSecret { get; }

    HttpClient CreateHttpClient(OdinId identity, FileSystemType fileSystemType = FileSystemType.Standard);
}

public static class ApiClientFactoryExtensions
{
    public static T CreateRefitHttpClient<T>(this IApiClientFactory factory, OdinId identity,
        FileSystemType fileSystemType = FileSystemType.Standard)
    {
        var client = factory.CreateHttpClient(identity, fileSystemType);
        var settings = new RefitSettings(new SharedSecretSystemTextJsonContentSerializer(factory.SharedSecret));
        return RestService.For<T>(client, settings);
    }
}