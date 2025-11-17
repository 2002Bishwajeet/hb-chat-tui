namespace Odin.ClientApi.Universal.Drive;

public class EncryptedAttachmentUploadResult
{
    public required string Key { get; init; }
    public required string ContentType { get; init; }
    public required string EncryptedContent64 { get; init; }
}