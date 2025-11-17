using Odin.Services.Drives.DriveCore.Storage;
using Odin.Services.Drives.FileSystem.Base.Upload;

namespace Odin.ClientApi.Universal.Drive;

public class UploadablePayloadDefinition
{
    public byte[] Iv { get; set; } = Guid.Empty.ToByteArray();
    public string Key { get; set; } = "";

    public string ContentType { get; set; } = "";

    public byte[] Content { get; set; } = [];

    public string DescriptorContent { get; set; } = "";

    public ThumbnailContent PreviewThumbnail { get; set; } = new();

    public List<ThumbnailContent> Thumbnails { get; set; } = new();

    public UploadManifestPayloadDescriptor ToPayloadDescriptor(
        PayloadUpdateOperationType updateOperationType = PayloadUpdateOperationType.None)
    {
        var t = this.Thumbnails?.Select(thumb => new UploadedManifestThumbnailDescriptor()
        {
            ThumbnailKey = $"{this.Key}{thumb.PixelWidth}{thumb.PixelHeight}", //hulk smash (it all together)
            PixelWidth = thumb.PixelWidth,
            PixelHeight = thumb.PixelHeight
        });

        return new UploadManifestPayloadDescriptor
        {
            Iv = this.Iv,
            PayloadKey = this.Key,
            DescriptorContent = this.DescriptorContent,
            PreviewThumbnail = this.PreviewThumbnail,
            Thumbnails = t,
            PayloadUpdateOperationType = updateOperationType,
            ContentType = this.ContentType
        };
    }
}