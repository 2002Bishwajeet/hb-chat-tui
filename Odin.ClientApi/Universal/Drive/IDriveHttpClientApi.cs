using Odin.Core;
using Odin.Hosting.Controllers;
using Odin.Hosting.Controllers.Base.Drive;
using Odin.Hosting.Controllers.Base.Drive.Status;
using Odin.Hosting.Controllers.ClientToken.Shared.Drive;
using Odin.Services.Apps;
using Odin.Services.Base.SharedTypes;
using Odin.Services.Drives;
using Odin.Services.Drives.FileSystem.Base.Update;
using Odin.Services.Drives.FileSystem.Base.Upload;
using Odin.Services.Drives.FileSystem.Base.Upload.Attachments;
using Odin.Services.Peer.Incoming.Drive.Transfer;
using Odin.Services.Peer.Outgoing.Drive.Transfer;
using Odin.Services.Peer.Outgoing.Drive.Transfer.Outbox;
using Refit;
using QueryModifiedRequest = Odin.Services.Drives.QueryModifiedRequest;

namespace Odin.ClientApi.Universal.Drive
{
    public interface IDriveHttpClientApi
    {
        private const string RootDriveEndpoint = "/drive";
        private const string RootStorageEndpoint = RootDriveEndpoint + "/files";
        private const string RootQueryEndpoint = RootDriveEndpoint + "/query";

        [Get(RootDriveEndpoint + "/metadata/type")]
        Task<ApiResponse<PagedResult<ClientDriveData>>> GetDrivesByType(Guid driveType, int pageNumber, int pageSize,
            CancellationToken cancellationToken);

        [Post("/transit/inbox/processor/process")]
        Task<ApiResponse<InboxStatus>> ProcessInbox([Body] ProcessInboxRequest request);

        [Multipart]
        [Post(RootStorageEndpoint + "/upload")]
        Task<ApiResponse<UploadResult>> UploadStream(StreamPart[] streamdata);

        [Multipart]
        [Patch(RootStorageEndpoint + "/update")]
        Task<ApiResponse<FileUpdateResult>> UpdateFile(StreamPart[] streamdata);

        [Patch(RootStorageEndpoint + "/update-local-metadata-tags")]
        Task<ApiResponse<UpdateLocalMetadataResult>> UpdateLocalMetadataTags([Body] UpdateLocalMetadataTagsRequest request);

        [Patch(RootStorageEndpoint + "/update-local-metadata-content")]
        Task<ApiResponse<UpdateLocalMetadataResult>> UpdateLocalMetadataContent([Body] UpdateLocalMetadataContentRequest request);

        [Post(RootStorageEndpoint + "/delete")]
        Task<ApiResponse<DeleteFileResult>> SoftDeleteFile([Body] DeleteFileRequest file);

        [Post(RootStorageEndpoint + "/deletefileidbatch")]
        Task<ApiResponse<DeleteFileIdBatchResult>> DeleteFileIdBatch([Body] DeleteFileIdBatchRequest request);

        [Post(RootStorageEndpoint + "/deletegroupidbatch")]
        Task<ApiResponse<DeleteFilesByGroupIdBatchResult>> DeleteFilesByGroupIdBatch([Body] DeleteFilesByGroupIdBatchRequest request);

        [Post(RootStorageEndpoint + "/payload")]
        Task<ApiResponse<HttpContent>> GetPayloadPost([Body] GetPayloadRequest request);

        [Post(RootStorageEndpoint + "/thumb")]
        Task<ApiResponse<HttpContent>> GetThumbnailPost([Body] GetThumbnailRequest request);

        [Get(RootStorageEndpoint + "/thumb")]
        Task<ApiResponse<HttpContent>> GetThumbnail(Guid fileId, Guid alias, Guid type, int width, int height);

        [Get(RootStorageEndpoint + "/payload")]
        Task<ApiResponse<HttpContent>> GetPayload(Guid fileId, Guid alias, Guid type);

        [Get(RootStorageEndpoint + "/header")]
        Task<ApiResponse<SharedSecretEncryptedFileHeader>> GetFileHeader(Guid fileId, Guid alias, Guid type);

        [Get(RootQueryEndpoint + "/specialized/cuid/header")]
        Task<ApiResponse<SharedSecretEncryptedFileHeader>> GetFileHeaderByClientUniqueId(Guid clientUniqueId, Guid alias, Guid type);

        [Get(RootQueryEndpoint + "/specialized/cuid/payload")]
        Task<ApiResponse<SharedSecretEncryptedFileHeader>>
            GetPayloadByClientUniqueId(Guid clientUniqueId, Guid alias, Guid type, string payloadKey);

        [Get(RootQueryEndpoint + "/specialized/cuid/thumb")]
        Task<ApiResponse<SharedSecretEncryptedFileHeader>> GetThumbnailByClientUniqueId(Guid clientUniqueId, Guid alias, Guid type,
            string payloadKey);

        [Post(RootQueryEndpoint + "/modified")]
        Task<ApiResponse<QueryModifiedResult>> GetModified([Body] QueryModifiedRequest request);

        [Post(RootQueryEndpoint + "/batch")]
        Task<ApiResponse<QueryBatchResponse>> GetBatch([Body] QueryBatchRequest request);

        [Get(RootStorageEndpoint + "/transfer-history")]
        Task<ApiResponse<FileTransferHistoryResponse>> GetTransferHistory(Guid fileId, Guid alias, Guid type);

        [Post(RootQueryEndpoint + "/batchcollection")]
        Task<ApiResponse<QueryBatchCollectionResponse>> GetBatchCollection([Body] QueryBatchCollectionRequest request);

        [Get(RootDriveEndpoint + "/status")]
        Task<ApiResponse<DriveStatus>> GetDriveStatus(Guid alias, Guid type);

        [Get(RootDriveEndpoint + "/outbox-item")]
        Task<ApiResponse<RedactedOutboxFileItem>> GetOutboxItem(Guid alias, Guid type, Guid fileId, string recipient);

        [Post(RootStorageEndpoint + "/send-read-receipt")]
        Task<ApiResponse<SendReadReceiptResult>> SendReadReceipt(SendReadReceiptRequest request);
    }
}