using Odin.ClientApi.Factory;
using Odin.Core;
using Refit;

namespace Odin.ClientApi.Utils;

public static class RefitCreator
{
    public static T RestServiceFor<T>(HttpClient client, byte[] sharedSecret)
    {
        return RestServiceFor<T>(client, sharedSecret.ToSensitiveByteArray());
    }

    /// <summary>
    /// Creates a Refit service using the shared secret encrypt/decrypt wrapper
    /// </summary>
    public static T RestServiceFor<T>(HttpClient client, SensitiveByteArray sharedSecret)
    {
        var settings = new RefitSettings(new SharedSecretSystemTextJsonContentSerializer(sharedSecret));
        return RestService.For<T>(client, settings);
    }
}