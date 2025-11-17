using Odin.ClientApi.App.Auth;
using Odin.ClientApi.App.Notifications;
using Odin.ClientApi.Universal.Drive;
using Odin.ClientApi.Universal.Follower;
using Odin.ClientApi.Universal.Peer.Direct;
using Odin.ClientApi.Universal.Peer.Query;
using Odin.Core.Identity;
using Odin.Services.Authorization.ExchangeGrants;

namespace Odin.ClientApi.App
{
    /// <summary>
    /// Api client for working with the /api/apps endpoints
    /// </summary>
    public class OdinApiAppClient
    {
        public OdinApiAppClient(OdinId identity, ClientAccessToken cat) : this(identity, cat.ToAuthenticationToken(),
            cat.SharedSecret.GetKey())
        {
        }

        public OdinApiAppClient(OdinId identity, ClientAuthenticationToken cat, byte[] sharedSecret)
        {
            OdinId = identity;

            var factory = new AppApiClientFactory(cat, sharedSecret);
            Auth = new AppAuthApiClient(identity, factory);
            AppNotifications = new AppNotificationsApiClient(identity, factory);
            Drive = new UniversalDriveApiClient(identity, factory);
            Reactions = new UniversalDriveReactionClient(identity, factory);
            PeerQuery = new UniversalPeerQueryApiClient(identity, factory);
            PeerDirect = new UniversalPeerDirectApiClient(identity, factory);
            StaticFilePublisher = new UniversalStaticFileApiClient(identity, factory);
            Follower = new UniversalFollowerApiClient(identity, factory);
        }

        public OdinId OdinId { get; }
        public AppAuthApiClient Auth { get; }
        public AppNotificationsApiClient AppNotifications { get; }
        public UniversalDriveApiClient Drive { get; }
        public UniversalDriveReactionClient Reactions { get; }
        public UniversalPeerQueryApiClient PeerQuery { get; }
        public UniversalStaticFileApiClient StaticFilePublisher { get; }
        public UniversalFollowerApiClient Follower { get; }
        public UniversalPeerDirectApiClient PeerDirect { get; }

        public async Task<bool> VerifyToken(CancellationToken cancellationToken = default)
        {
            var response = await this.Auth.VerifyToken(cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}