using System.Diagnostics;
using Odin.ClientApi.Universal.Drive;
using Odin.Core;
using Odin.Core.Identity;
using Odin.Hosting.Controllers;
using Odin.Services.Drives;
using Odin.Services.Membership.Connections.Requests;
using Refit;

namespace Odin.ClientApi.Universal.Connections;

public class UniversalCircleNetworkRequestsApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<HttpContent>> AcceptConnectionRequest(OdinId sender, IEnumerable<GuidId>? circleIdsGrantedToSender = null)
    {
        // Accept the request
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        {
            var header = new AcceptRequestHeader()
            {
                Sender = sender,
                CircleIds = circleIdsGrantedToSender,
                ContactData = new ContactRequestData()
            };

            var acceptResponse = await svc.AcceptConnectionRequest(header);
            return acceptResponse;
        }
    }

    public async Task<ApiResponse<ConnectionRequestResponse>> GetIncomingRequestFrom(OdinId sender)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);

        var response = await svc.GetPendingRequest(new OdinIdRequest() { OdinId = sender });

        return response;
    }

    public async Task<ApiResponse<ConnectionRequestResponse>> GetOutgoingSentRequestTo(OdinId recipient)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);

        var response = await svc.GetSentRequest(new OdinIdRequest() { OdinId = recipient });

        return response;
    }

    public async Task<ApiResponse<HttpContent>> DeleteConnectionRequestFrom(OdinId sender)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);


        var deleteResponse = await svc.DeletePendingRequest(new OdinIdRequest() { OdinId = sender });
        return deleteResponse;
    }

    public async Task<ApiResponse<HttpContent>> DeleteSentRequestTo(OdinId recipient)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        var deleteResponse = await svc.DeleteSentRequest(new OdinIdRequest() { OdinId = recipient });
        return deleteResponse;
    }

    public async Task<ApiResponse<HttpContent>> SendConnectionRequest(OdinId recipient,
        IEnumerable<GuidId>? circlesGrantedToRecipient = null)
    {
        // Send the request
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        var id = Guid.NewGuid();
        var requestHeader = new ConnectionRequestHeader()
        {
            Id = id,
            Recipient = recipient,
            Message = "Please add me",
            ContactData = new ContactRequestData()
            {
                Name = "Test Test"
            },
            CircleIds = circlesGrantedToRecipient?.ToList()
        };

        var response = await svc.SendConnectionRequest(requestHeader);
        return response;
    }

    public async Task<ApiResponse<IntroductionResult>> SendIntroductions(IntroductionGroup group)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        return await svc.SendIntroductions(group);
    }

    public async Task<ApiResponse<HttpContent>> ProcessIncomingIntroductions()
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        return await svc.ProcessIncomingIntroductions();
    }

    public async Task<ApiResponse<HttpContent>> AutoAcceptEligibleIntroductions()
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        return await svc.AutoAcceptEligibleIntroductions();
    }

    public async Task<ApiResponse<List<IdentityIntroduction>>> GetReceivedIntroductions()
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        return await svc.GetReceivedIntroductions();
    }

    public async Task<ApiResponse<HttpContent>> DeleteAllIntroductions()
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkRequests>(identity);
        return await svc.DeleteAllIntroductions();
    }

    public async Task<ApiResponse<HttpContent>> DisconnectFrom(OdinId recipient)
    {
        var svc = factory.CreateRefitHttpClient<IRefitUniversalCircleNetworkConnections>(identity);
        var disconnectResponse = await svc.Disconnect(new OdinIdRequest() { OdinId = recipient });
        return disconnectResponse;
    }

}