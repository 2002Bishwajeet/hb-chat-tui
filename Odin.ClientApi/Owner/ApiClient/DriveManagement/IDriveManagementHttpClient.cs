using Odin.Core;
using Odin.Hosting.Controllers.OwnerToken.Drive;
using Odin.Services.Authentication.Owner;
using Odin.Services.Drives.Management;
using Refit;

namespace Odin.ClientApi.Owner.ApiClient.DriveManagement
{
    public interface IRefitDriveManagement
    {
        private const string RootEndpoint = OwnerApiPathConstants.DriveManagementV1;

        [Post(RootEndpoint + "/create")]
        Task<ApiResponse<bool>> CreateDrive([Body] CreateDriveRequest request);

        [Post(RootEndpoint)]
        Task<ApiResponse<PagedResult<OwnerClientDriveData>>> GetDrives([Body] GetDrivesRequest request);

    }
}