using Odin.ClientApi.Universal.Drive;
using Odin.ClientApi.Utils;
using Odin.Core;
using Odin.Core.Identity;
using Odin.Core.Serialization;
using Odin.Core.Storage;
using Odin.Hosting.Controllers.Base.Drive;
using Odin.Hosting.Controllers.Base.Transit;
using Odin.Services.Drives;
using Odin.Services.Drives.FileSystem.Base.Upload;
using Odin.Services.Peer.Encryption;
using Odin.Services.Peer.Outgoing.Drive;
using Refit;

namespace Odin.ClientApi.Universal.Peer.Direct;

public class UniversalPeerDirectApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<TransitResult>> TransferMetadata(
        TargetDrive remoteTargetDrive,
        UploadFileMetadata fileMetadata,
        List<string> recipients,
        Guid? overwriteGlobalTransitFileId,
        FileSystemType fileSystemType = FileSystemType.Standard)
    {
        var transferIv = ByteArrayUtil.GetRndByteArray(16);
        var keyHeader = KeyHeader.NewRandom16();

        TransitInstructionSet instructionSet = new TransitInstructionSet()
        {
            TransferIv = transferIv,
            OverwriteGlobalTransitFileId = overwriteGlobalTransitFileId,
            RemoteTargetDrive = remoteTargetDrive,
            Recipients = recipients,
        };

        var instructionStream = new MemoryStream(OdinSystemSerializer.Serialize(instructionSet).ToUtf8ByteArray());

        fileMetadata.IsEncrypted = false;

        var sharedSecret = factory.SharedSecret;
        var descriptor = new UploadFileDescriptor()
        {
            EncryptedKeyHeader = EncryptedKeyHeader.EncryptKeyHeaderAes(keyHeader, instructionSet.TransferIv, ref sharedSecret),
            FileMetadata = fileMetadata
        };

        var fileDescriptorCipher = Utils.MiscUtils.JsonEncryptAes(descriptor, instructionSet.TransferIv, sharedSecret);

        List<StreamPart> parts = new()
        {
            new StreamPart(instructionStream, "instructionSet.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Instructions)),
            new StreamPart(fileDescriptorCipher, "fileDescriptor.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Metadata))
        };

        var svc = factory.CreateRefitHttpClient<IUniversalRefitPeerDirect>(identity, fileSystemType);
        ApiResponse<TransitResult> response = await svc.UploadFile(parts.ToArray());

        keyHeader.AesKey.Wipe();

        return response;
    }

    public async Task<ApiResponse<TransitResult>> TransferNewFile(
        TargetDrive remoteTargetDrive,
        UploadFileMetadata fileMetadata,
        List<OdinId> recipients,
        Guid? overwriteGlobalTransitFileId,
        UploadManifest uploadManifest,
        List<UploadablePayloadDefinition> payloads,
        FileSystemType fileSystemType = FileSystemType.Standard)
    {
        var transferIv = ByteArrayUtil.GetRndByteArray(16);
        var keyHeader = KeyHeader.NewRandom16();

        TransitInstructionSet instructionSet = new TransitInstructionSet()
        {
            TransferIv = transferIv,
            OverwriteGlobalTransitFileId = overwriteGlobalTransitFileId,
            RemoteTargetDrive = remoteTargetDrive,
            Recipients = recipients.Select(d => d.DomainName).ToList(),
            Manifest = uploadManifest
        };

        var instructionStream = new MemoryStream(OdinSystemSerializer.Serialize(instructionSet).ToUtf8ByteArray());

        var sharedSecret = factory.SharedSecret;
        var descriptor = new UploadFileDescriptor()
        {
            EncryptedKeyHeader = EncryptedKeyHeader.EncryptKeyHeaderAes(keyHeader, instructionSet.TransferIv, ref sharedSecret),
            FileMetadata = fileMetadata
        };


        var fileDescriptorCipher = Utils.MiscUtils.JsonEncryptAes(descriptor, instructionSet.TransferIv, sharedSecret);

        List<StreamPart> parts =
        [
            new StreamPart(instructionStream, "instructionSet.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Instructions)),
            new StreamPart(fileDescriptorCipher, "fileDescriptor.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Metadata))
        ];

        foreach (var payloadDefinition in payloads)
        {
            parts.Add(new StreamPart(new MemoryStream(payloadDefinition.Content), payloadDefinition.Key, payloadDefinition.ContentType,
                Enum.GetName(MultipartUploadParts.Payload)));

            foreach (var thumbnail in payloadDefinition.Thumbnails)
            {
                var thumbnailKey = $"{payloadDefinition.Key}{thumbnail.PixelWidth}{thumbnail.PixelHeight}"; //hulk smash (it all together)
                parts.Add(new StreamPart(new MemoryStream(thumbnail.Content), thumbnailKey, thumbnail.ContentType,
                    Enum.GetName(MultipartUploadParts.Thumbnail)));
            }
        }

        var svc = factory.CreateRefitHttpClient<IUniversalRefitPeerDirect>(identity);
        ApiResponse<TransitResult> response = await svc.UploadFile(parts.ToArray());

        keyHeader.AesKey.Wipe();

        return response;
    }


    public async Task<(ApiResponse<TransitResult> response, string encryptedMetadataContent64)> TransferNewEncryptedFile(
        TargetDrive remoteTargetDrive,
        UploadFileMetadata fileMetadata,
        List<OdinId> recipients,
        Guid? overwriteGlobalTransitFileId,
        UploadManifest uploadManifest,
        List<UploadablePayloadDefinition> payloads,
        AppNotificationOptions? notificationOptions = null,
        KeyHeader? keyHeader = null,
        FileSystemType fileSystemType = FileSystemType.Standard)
    {
        var transferIv = ByteArrayUtil.GetRndByteArray(16);
        keyHeader = keyHeader ?? KeyHeader.NewRandom16();

        TransitInstructionSet instructionSet = new TransitInstructionSet()
        {
            TransferIv = transferIv,
            OverwriteGlobalTransitFileId = overwriteGlobalTransitFileId,
            RemoteTargetDrive = remoteTargetDrive,
            Recipients = recipients.Select(d => d.DomainName).ToList(),
            Manifest = uploadManifest,
            NotificationOptions = notificationOptions
        };

        var encryptedJsonContent64 = keyHeader.EncryptDataAes(fileMetadata.AppData.Content.ToUtf8ByteArray()).ToBase64();
        fileMetadata.AppData.Content = encryptedJsonContent64;
        fileMetadata.IsEncrypted = true;

        var instructionStream = new MemoryStream(OdinSystemSerializer.Serialize(instructionSet).ToUtf8ByteArray());

        var sharedSecret = factory.SharedSecret;
        var descriptor = new UploadFileDescriptor()
        {
            EncryptedKeyHeader = EncryptedKeyHeader.EncryptKeyHeaderAes(keyHeader, instructionSet.TransferIv, ref sharedSecret),
            FileMetadata = fileMetadata
        };

        var fileDescriptorCipher = Utils.MiscUtils.JsonEncryptAes(descriptor, instructionSet.TransferIv, sharedSecret);

        List<StreamPart> parts =
        [
            new StreamPart(instructionStream, "instructionSet.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Instructions)),
            new StreamPart(fileDescriptorCipher, "fileDescriptor.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Metadata))
        ];

        foreach (var payloadDefinition in payloads)
        {
            var pc = keyHeader.EncryptDataAesAsStream(payloadDefinition.Content);
            parts.Add(new StreamPart(pc, payloadDefinition.Key, payloadDefinition.ContentType,
                Enum.GetName(MultipartUploadParts.Payload)));

            foreach (var thumbnail in payloadDefinition.Thumbnails)
            {
                var thumbnailKey = $"{payloadDefinition.Key}{thumbnail.PixelWidth}{thumbnail.PixelHeight}"; //hulk smash (it all together)
                var tc = keyHeader.EncryptDataAesAsStream(thumbnail.Content);
                parts.Add(new StreamPart(tc, thumbnailKey, thumbnail.ContentType, Enum.GetName(MultipartUploadParts.Thumbnail)));
            }
        }

        var svc = factory.CreateRefitHttpClient<IUniversalRefitPeerDirect>(identity);
        ApiResponse<TransitResult> response = await svc.UploadFile(parts.ToArray());


        return (response, encryptedJsonContent64);
    }


    public async Task<ApiResponse<DeleteFileResult>> DeleteFile(FileSystemType fileSystemType, GlobalTransitIdFileIdentifier remoteGlobalTransitIdFileIdentifier,
        List<OdinId> recipients)
    {
        var request = new DeleteFileByGlobalTransitIdRequest()
        {
            FileSystemType = fileSystemType,
            GlobalTransitIdFileIdentifier = remoteGlobalTransitIdFileIdentifier,
            Recipients = recipients.Select(d => d.DomainName).ToList(),
        };

        var svc = factory.CreateRefitHttpClient<IUniversalRefitPeerDirect>(identity);
        var response = await svc.SendDeleteRequest(request);
        return response;
    }

    public async Task<(ApiResponse<TransitResult> transitResultResponse, string encryptedJsonContent64)> TransferEncryptedMetadata(
        TargetDrive remoteTargetDrive,
        UploadFileMetadata fileMetadata,
        List<string> recipients,
        Guid? overwriteGlobalTransitFileId,
        FileSystemType fileSystemType = FileSystemType.Standard)
    {
        var transferIv = ByteArrayUtil.GetRndByteArray(16);
        var keyHeader = KeyHeader.NewRandom16();

        TransitInstructionSet instructionSet = new TransitInstructionSet()
        {
            TransferIv = transferIv,
            OverwriteGlobalTransitFileId = overwriteGlobalTransitFileId,
            RemoteTargetDrive = remoteTargetDrive,
            Recipients = recipients,
        };

        var instructionStream = new MemoryStream(OdinSystemSerializer.Serialize(instructionSet).ToUtf8ByteArray());

        var encryptedJsonContent64 = keyHeader.EncryptDataAes(fileMetadata.AppData.Content.ToUtf8ByteArray()).ToBase64();
        fileMetadata.AppData.Content = encryptedJsonContent64;
        fileMetadata.IsEncrypted = true;

        var sharedSecret = factory.SharedSecret;
        var descriptor = new UploadFileDescriptor()
        {
            EncryptedKeyHeader = EncryptedKeyHeader.EncryptKeyHeaderAes(keyHeader, instructionSet.TransferIv, ref sharedSecret),
            FileMetadata = fileMetadata
        };

        var fileDescriptorCipher = Utils.MiscUtils.JsonEncryptAes(descriptor, instructionSet.TransferIv, factory.SharedSecret);

        List<StreamPart> parts = new()
        {
            new StreamPart(instructionStream, "instructionSet.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Instructions)),
            new StreamPart(fileDescriptorCipher, "fileDescriptor.encrypted", "application/json",
                Enum.GetName(MultipartUploadParts.Metadata)),
        };

        var svc = factory.CreateRefitHttpClient<IUniversalRefitPeerDirect>(identity, fileSystemType);

        ApiResponse<TransitResult> transitResultResponse = await svc.UploadFile(parts.ToArray());

        keyHeader.AesKey.Wipe();

        return (transitResultResponse, encryptedJsonContent64);
    }
}