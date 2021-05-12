using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;

namespace TGS.Shared.Game.ApiClients.VR
{
    /// <summary>
    /// VR Utility
    /// </summary>
    public static class VRUtility
    {
        private static readonly Dictionary<GPAction, string> _MapAction;

        static VRUtility()
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

        /// <summary>
        /// Gets the action URL.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static string GetAction(GPAction action)
        {
            return _MapAction.Where(x => x.Key == action).FirstOrDefault().Value;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null)
        {
            var value = JsonSerializer.Serialize(request, options);
            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(value, options);
            return string.Join("&", items.Where(x => x.Value != null).Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key, bool isEncodeUrl = true)
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

        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key, bool isDecodeUrl = true)
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



    }
}
