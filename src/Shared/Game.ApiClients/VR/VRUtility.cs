using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;

namespace TGS.Shared.Game.ApiClients.VR
{
    public interface IVRUtility
    {
        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public string GetAction(GPAction action);

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null);

        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        public string AesEncrypt(string str, string key, bool isEncodeUrl = true);

        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        public string AesDecrypt(string str, string key, bool isDecodeUrl = true);
    }

    /// <summary>
    /// VR Utility
    /// </summary>
    internal class VRUtility : IVRUtility
    {
        private readonly Dictionary<GPAction, string> _MapAction;

        public VRUtility()
        {
            _MapAction = new Dictionary<GPAction, string>();
            _MapAction.Add(GPAction.GetGameUrl, "/Account/LoginValidate");
            _MapAction.Add(GPAction.CreatePlayer, "/Account/CreateUser");
            _MapAction.Add(GPAction.GetBalance, "/UserWallet/Balance");
            _MapAction.Add(GPAction.FundOut, "/UserWallet/Transaction");
            _MapAction.Add(GPAction.FundIn, "/UserWallet/Transaction");
            _MapAction.Add(GPAction.FundChecking, "/UserWallet/TransactionRecord");
            _MapAction.Add(GPAction.GameRecord, "/MerchantQuery/Bet");
        }

        public string GetAction(GPAction action)
        {
            return _MapAction.Where(x => x.Key == action).FirstOrDefault().Value;
        }

        public string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null)
        {
            var value = JsonSerializer.Serialize(request, options);
            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(value, options);
            return string.Join("&", items.Where(x => x.Value != null).Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        public string AesEncrypt(string str, string key, bool isEncodeUrl = true)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            var base64Str = Convert.ToBase64String(resultArray, 0, resultArray.Length);

            return isEncodeUrl ? System.Web.HttpUtility.UrlEncode(base64Str) : base64Str;
        }

        public string AesDecrypt(string str, string key, bool isDecodeUrl = true)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return null;
                if (isDecodeUrl)
                    str = System.Web.HttpUtility.UrlDecode(str);

                Byte[] toEncryptArray = Convert.FromBase64String(str);

                System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = System.Security.Cryptography.CipherMode.ECB,
                    Padding = System.Security.Cryptography.PaddingMode.PKCS7
                };

                System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return null;
            }
        }
    }
}
