using Odin.Hosting.Controllers.Base.Drive.GroupReactions;
using Odin.Services.Drives.Reactions;
using Odin.Services.Drives.Reactions.Redux.Group;
using Refit;

namespace Odin.ClientApi.Universal.Drive
{
    public interface IUniversalDriveReactionHttpClient
    {
        private const string ReactionRootEndpoint = "/drive/files/group/reactions";

        [Post(ReactionRootEndpoint)]
        Task<ApiResponse<AddReactionResult>> AddReaction([Body] AddReactionRequestRedux request);

        [Delete(ReactionRootEndpoint)]
        Task<ApiResponse<DeleteReactionResult>> DeleteReaction([Body] DeleteReactionRequestRedux request);


        [Get(ReactionRootEndpoint)]
        Task<ApiResponse<GetReactionsResponse>> GetReactions([Query] GetReactionsRequestRedux request);

        [Get(ReactionRootEndpoint + "/summary")]
        Task<ApiResponse<GetReactionCountsResponse>> GetReactionCountsByFile([Query] GetReactionsRequestRedux request);

        [Get(ReactionRootEndpoint + "/by-identity")]
        Task<ApiResponse<List<string>>> GetReactionsByIdentity([Query] GetReactionsByIdentityRequestRedux request);
    }
}