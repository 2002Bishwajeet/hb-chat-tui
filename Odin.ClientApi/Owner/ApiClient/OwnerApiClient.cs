using System.Diagnostics;
using System.Net;
using System.Web;
using Odin.ClientApi.Owner.ApiClient.AppManagement;
using Odin.ClientApi.Owner.ApiClient.Configuration;
using Odin.ClientApi.Owner.ApiClient.DriveManagement;
using Odin.Core;
using Odin.Core.Cryptography.Crypto;
using Odin.Core.Cryptography.Data;
using Odin.Core.Cryptography.Login;
using Odin.Core.Identity;
using Odin.Services.Authentication.Owner;
using Odin.Services.Authorization.ExchangeGrants;
using Refit;

namespace Odin.ClientApi.Owner.ApiClient
{
    [DebuggerDisplay("Owner api client for {OdinId}")]
    public class OwnerApiClient(OdinId identity, IApiClientFactory factory)
    {
        public OdinId OdinId => identity;

        public AppManagementApiClient AppManager { get; } = new(identity, factory);

        public DriveManagementApiClient DriveManager { get; } = new(identity, factory);

        public OwnerConfigurationApiClient Configuration { get; } = new(identity, factory);
        
        public static async Task<(ClientAuthenticationToken cat, SensitiveByteArray sharedSecret)> LoginToOwnerConsole(OdinId identity,
            string password, int? httpPort = null)
        {
            var clientEccFullKey = new EccFullKeyData(EccKeyListManagement.zeroSensitiveKey, EccKeySize.P384, 1);
            return await LoginToOwnerConsole(identity, password, clientEccFullKey, httpPort);
        }

        public static async Task<(ClientAuthenticationToken cat, SensitiveByteArray sharedSecret)> LoginToOwnerConsole(OdinId identity,
            string password, EccFullKeyData clientEccFullKey, int? httpPort = null)
        {
            var handler = new HttpClientHandler();
            var jar = new CookieContainer();
            handler.CookieContainer = jar;
            handler.UseCookies = true;

            using HttpClient authClient = new(handler);
            authClient.BaseAddress = new Uri($"https://{identity.DomainName}:{httpPort}");
            var svc = RestService.For<IOwnerAuthenticationClient>(authClient);

            var reply = await CalculateAuthenticationPasswordReply(authClient, password, clientEccFullKey);
            var response = await svc.Authenticate(reply);

            var ownerAuthenticationResult = response.Content!;

            var cookies = jar.GetCookies(authClient.BaseAddress);
            var tokenCookie = HttpUtility.UrlDecode(cookies[OwnerAuthConstants.CookieName]?.Value);

            var result = ClientAuthenticationToken.Parse(tokenCookie);
            return (result, ownerAuthenticationResult.SharedSecret.ToSensitiveByteArray());
        }

        /// <summary>
        /// Creates a password reply for use when you are authenticating as the owner
        /// </summary>
        private static async Task<PasswordReply> CalculateAuthenticationPasswordReply(HttpClient authClient, string password,
            EccFullKeyData clientEccFullKey)
        {
            var svc = RestService.For<IOwnerAuthenticationClient>(authClient);

            var nonceResponse = await svc.GenerateAuthenticationNonce();
            var clientNonce = nonceResponse.Content;

            var nonce = new NonceData(clientNonce!.SaltPassword64, clientNonce.SaltKek64, clientNonce.PublicJwk, clientNonce.CRC)
            {
                Nonce64 = clientNonce.Nonce64
            };

            var reply = PasswordDataManager.CalculatePasswordReply(password, nonce, clientEccFullKey);
            return reply;
        }
    }
}