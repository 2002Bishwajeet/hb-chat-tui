using Odin.Core;
using Odin.Core.Identity;
using Odin.Hosting.Controllers.OwnerToken.Drive;
using Odin.Services.Drives;
using Odin.Services.Drives.Management;
using Refit;

namespace Odin.ClientApi.Owner.ApiClient.DriveManagement;

public class DriveManagementApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<bool>> CreateDrive(TargetDrive targetDrive, string name, string metadata, bool allowAnonymousReads,
        bool ownerOnly = false,
        bool allowSubscriptions = false, Dictionary<string, string>? attributes = null)
    {
        var svc = factory.CreateRefitHttpClient<IRefitDriveManagement>(identity);

        if (ownerOnly && allowAnonymousReads)
        {
            throw new Exception("cannot have an owner only drive that allows anonymous reads");
        }

        var response = await svc.CreateDrive(new CreateDriveRequest()
        {
            TargetDrive = targetDrive,
            Name = name,
            Metadata = metadata,
            AllowAnonymousReads = allowAnonymousReads,
            AllowSubscriptions = allowSubscriptions,
            OwnerOnly = ownerOnly,
            Attributes = attributes
        });

        return response;
    }

    public async Task<ApiResponse<PagedResult<OwnerClientDriveData>>> GetDrives(int pageNumber = 1, int pageSize = 100)
    {
        var svc = factory.CreateRefitHttpClient<IRefitDriveManagement>(identity);
        return await svc.GetDrives(new GetDrivesRequest() { PageNumber = pageNumber, PageSize = pageSize });
    }
}