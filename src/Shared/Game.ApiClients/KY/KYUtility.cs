using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;

namespace TGS.Shared.Game.ApiClients.KY
{
    /// <summary>
    /// KY Utility
    /// </summary>
    public static class KYUtility
    {
        private static readonly Dictionary<GPAction, int> _MapAction;

        /// <summary>
        /// Initializes the <see cref="KYUtility"/> class.
        /// </summary>
        static KYUtility()
        {
            _MapAction = new Dictionary<GPAction, int>();
            _MapAction.Add(GPAction.GetGameUrl, 0);
            _MapAction.Add(GPAction.GetBalance, 7);
            _MapAction.Add(GPAction.FundOut, 2);
            _MapAction.Add(GPAction.FundIn, 3);
            _MapAction.Add(GPAction.FundChecking, 4);
            _MapAction.Add(GPAction.Kick, 8);
            _MapAction.Add(GPAction.GameRecord, 6);
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static int? GetAction(GPAction action)
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
            return string.Join("&", items.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
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
