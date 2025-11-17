using System.Collections.Specialized;
using System.Web;
using Odin.Core;
using Odin.Core.Cryptography.Crypto;
using Odin.Core.Exceptions;
using Odin.Core.Serialization;
using Odin.Services.Authorization.ExchangeGrants;
using Odin.Services.Authorization.Permissions;
using Odin.Services.Base;
using Odin.Services.Drives;
using Odin.Services.Peer.Encryption;
using Refit;

namespace Odin.ClientApi.Utils
{
    public static class MiscUtils
    {
        public static OdinClientErrorCode ParseProblemDetails(ApiException apiException)
        {
            if (apiException.Content == null)
            {
                throw new Exception("Problem details content empty");
            }
            
            var pd = OdinSystemSerializer.Deserialize<ProblemDetails>(apiException.Content!);
            if (null == pd)
            {
                throw new Exception("Problem details not found");
            }
            var codeText = pd.Extensions["errorCode"].ToString();
            var code = Enum.Parse<OdinClientErrorCode>(codeText!, true);
            return code;
        }

        public static Stream GetEncryptedStream(string data, KeyHeader keyHeader)
        {
            var key = keyHeader.AesKey;
            var cipher = AesCbc.Encrypt(
                data: System.Text.Encoding.UTF8.GetBytes(data),
                key: key,
                iv: keyHeader.Iv);

            return new MemoryStream(cipher);
        }

        public static Stream EncryptAes(string data, byte[] iv, SensitiveByteArray key)
        {
            var cipher = AesCbc.Encrypt(
                data: System.Text.Encoding.UTF8.GetBytes(data),
                key: key,
                iv: iv);

            return new MemoryStream(cipher);
        }

        /// <summary>
        /// Converts data to json then encrypts
        /// </summary>
        public static Stream JsonEncryptAes(object instance, byte[] iv, SensitiveByteArray key)
        {
            var data = OdinSystemSerializer.Serialize(instance);

            var cipher = AesCbc.Encrypt(
                data: System.Text.Encoding.UTF8.GetBytes(data),
                key: key,
                iv: iv);

            return new MemoryStream(cipher);
        }

        public static PermissionSetGrantRequest CreatePermissionGrantRequest(TargetDrive targetDrive, DrivePermission drivePermissions)
        {
            return new PermissionSetGrantRequest
            {
                Drives =
                [
                    new DriveGrantRequest
                    {
                        PermissionedDrive = new()
                        {
                            Drive = targetDrive,
                            Permission = drivePermissions
                        }
                    }
                ],
                PermissionSet = new PermissionSet()
            };
        }
    }
}