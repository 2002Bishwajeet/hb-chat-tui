using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Odin.Core.Exceptions;

namespace Odin.ClientApi.Utils;

public static class UrlUtils
{
    public static bool IsRelativeUrl(string input)
    {
        return Uri.TryCreate(input, UriKind.Relative, out var uriResult)
               && !uriResult.IsAbsoluteUri;
    }

    public static string BuildRedirectUrl(HttpContext httpContext, string relativePath, IDictionary<string, string> queryParams)
    {
        if (!IsRelativeUrl(relativePath))
        {
            throw new OdinClientException("relativePath must be relative");
        }

        var request = httpContext.Request;

        string scheme = request.Scheme;
        string host = request.Host.Host;
        int? port = request.Host.Port;

        string baseUrl = port.HasValue
            ? $"{scheme}://{host}:{port}"
            : $"{scheme}://{host}";

        // Parse the relative path to extract existing query parameters
        var pathParts = relativePath.Split('?', 2);
        var pathWithoutQuery = pathParts[0];
        var existingQueryString = pathParts.Length > 1 ? pathParts[1] : string.Empty;

        var absoluteUri = new Uri(new Uri(baseUrl), pathWithoutQuery);

        // Merge existing query parameters with new ones
        var mergedParams = new Dictionary<string, string>();

        // Add existing query parameters from relativePath
        if (!string.IsNullOrEmpty(existingQueryString))
        {
            var existingParams = QueryHelpers.ParseQuery(existingQueryString);
            foreach (var kvp in existingParams)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    // Take the first value if multiple values exist for the same key
                    var value = kvp.Value.FirstOrDefault();
                    if (!string.IsNullOrEmpty(value))
                    {
                        mergedParams[kvp.Key] = value;
                    }
                }
            }
        }

        // Add/override with new query parameters
        if (queryParams != null)
        {
            foreach (var kvp in queryParams)
            {
                mergedParams[kvp.Key] = kvp.Value;
            }
        }

        var uriBuilder = new UriBuilder(absoluteUri)
        {
            Query = BuildQueryString(mergedParams)
        };

        return uriBuilder.ToString();
    }

    private static string BuildQueryString(IDictionary<string, string> parameters)
    {
        var encoded = parameters
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key))
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");

        return string.Join("&", encoded);
    }
}