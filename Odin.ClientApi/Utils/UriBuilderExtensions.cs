using System.Collections.Specialized;
using System.Web;

namespace Odin.ClientApi.Utils;

public static class UriBuilderExtensions
{
    public static void AddQueryParameters(this UriBuilder builder, NameValueCollection parametersToAdd)
    {
        var query = HttpUtility.ParseQueryString(builder.Query);

        foreach (string key in parametersToAdd)
        {
            query[key] = parametersToAdd[key];
        }

        builder.Query = query.ToString(); // re-encode query string
    }
}