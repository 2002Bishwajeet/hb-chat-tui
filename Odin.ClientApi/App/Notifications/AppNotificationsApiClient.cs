using Odin.Core.Identity;
using Odin.Services.AppNotifications.Data;
using Odin.Services.Peer.Outgoing.Drive;
using Refit;

namespace Odin.ClientApi.App.Notifications;

public class AppNotificationsApiClient(OdinId identity, IApiClientFactory factory)
{
    public async Task<ApiResponse<AddNotificationResult>> AddNotification(AppNotificationOptions options)
    {
        var svc = factory.CreateRefitHttpClient<IRefitNotifications>(identity);
        var response = await svc.AddNotification(new AddNotificationRequest()
        {
            AppNotificationOptions = options
        });

        return response;
    }

    public async Task<ApiResponse<NotificationsCountResult>> GetUnreadCounts()
    {
        var svc = factory.CreateRefitHttpClient<IRefitNotifications>(identity);
        var response = await svc.GetUnreadCounts();
        return response;
    }

    public async Task<ApiResponse<NotificationsListResult>> GetList(int count, string? cursor = null)
    {
        var svc = factory.CreateRefitHttpClient<IRefitNotifications>(identity);
        var response = await svc.GetList(count, cursor);
        return response;
    }

    public async Task<ApiResponse<HttpContent>> Update(List<UpdateNotificationRequest> updates)
    {
        var svc = factory.CreateRefitHttpClient<IRefitNotifications>(identity);
        var response = await svc.Update(new UpdateNotificationListRequest()
        {
            Updates = updates
        });
        return response;
    }

    public async Task<ApiResponse<HttpContent>> MarkReadByAppId(Guid appId)
    {
        var svc = factory.CreateRefitHttpClient<IRefitNotifications>(identity);
        var response = await svc.MarkReadByAppId(appId);
        return response;
    }

    public async Task<ApiResponse<HttpContent>> Delete(List<Guid> idList)
    {
        var svc = factory.CreateRefitHttpClient<IRefitNotifications>(identity);
        var response = await svc.DeleteNotification(new DeleteNotificationsRequest()
        {
            IdList = idList
        });
        return response;
    }
}