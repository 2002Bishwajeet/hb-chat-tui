using System.Text.Json.Serialization;
using Odin.Services.Drives;

namespace Odin.ClientApi.App.Auth.YouAuth;

/// <summary>
/// Parameters used when authorizing a new app to Homebae
/// </summary>
public class AppAuthorizationParams
{
    [JsonPropertyName("n")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string AppName { get; set; } = default!;

    [JsonPropertyName("appId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string AppId { get; set; } = default!;

    [JsonPropertyName("fn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string ClientFriendlyName { get; set; } = default!;

    [JsonPropertyName("p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? PermissionKeysCsv { get; set; }

    [JsonPropertyName("cp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? CirclePermissionKeysCsv { get; set; }

    [JsonPropertyName("d")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? DriveAccessJson { get; set; }

    [JsonPropertyName("cd")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? CircleDriveAccessJson { get; set; }

    [JsonPropertyName("c")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? CircleIdsCsv { get; set; }

    [JsonPropertyName("return")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string ReturnBehavior { get; set; } = default!;

    [JsonPropertyName("o")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? AppCorsOrigin { get; set; }
}

public class DriveParam
{
    [JsonPropertyName("a")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? DriveAlias { get; set; }

    [JsonPropertyName("t")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? DriveType { get; set; }

    [JsonPropertyName("n")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Name { get; set; }

    [JsonPropertyName("d")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Description { get; set; }

    [JsonPropertyName("p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public int DrivePermission { get; set; }

    [JsonPropertyName("r")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowAnonymousReads { get; set; }

    [JsonPropertyName("s")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowSubscriptions { get; set; }
}
