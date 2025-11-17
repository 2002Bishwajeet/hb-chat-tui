using Odin.Core.Identity;
using Odin.Core.Storage;
using Odin.Hosting.Controllers.Base.Drive.GroupReactions;
using Odin.Services.Drives.Reactions;
using Odin.Services.Drives.Reactions.Redux.Group;
using Refit;

namespace Odin.ClientApi.Universal.Drive;

public class UniversalDriveReactionClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<AddReactionResult>> AddReaction(AddReactionRequestRedux request)
    {
        var svc = factory.CreateRefitHttpClient<IUniversalDriveReactionHttpClient>(identity);
        var response = await svc.AddReaction(request);
        return response;
    }

    public async Task<ApiResponse<GetReactionsResponse>> GetReactions(GetReactionsRequestRedux request, FileSystemType fileSystemType = FileSystemType.Standard)
    {
        
        var svc = factory.CreateRefitHttpClient<IUniversalDriveReactionHttpClient>(identity, fileSystemType);
        var response = await svc.GetReactions(request);
        return response;
    }

    public async Task<ApiResponse<DeleteReactionResult>> DeleteReaction(DeleteReactionRequestRedux request)
    {
        var svc = factory.CreateRefitHttpClient<IUniversalDriveReactionHttpClient>(identity);
        var response = await svc.DeleteReaction(request);

        return response;
    }

    public async Task<ApiResponse<GetReactionCountsResponse>> GetReactionCountsByFile(GetReactionsRequestRedux request)
    {
        var svc = factory.CreateRefitHttpClient<IUniversalDriveReactionHttpClient>(identity);
        var response = await svc.GetReactionCountsByFile(request);
        return response;
    }

    public async Task<ApiResponse<List<string>>> GetReactionsByIdentity(GetReactionsByIdentityRequestRedux request)
    {

        var svc = factory.CreateRefitHttpClient<IUniversalDriveReactionHttpClient>(identity);
        var response = await svc.GetReactionsByIdentity(request);

        return response;
    }
}