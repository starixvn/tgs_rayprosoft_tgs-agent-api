using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;

namespace TGS.Shared.Game.ApiClients.KY
{
    public interface IKYUtility
    {
        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        int? GetAction(GPAction action);

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null);

        /// <summary>
        /// Hashes the m d5.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public string HashMD5(string data);

        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        string AesEncrypt(string data, string key, bool isEncodeUrl = true);

        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        string AesDecrypt(string data, string key, bool isDecodeUrl = true);
    }

    internal class KYUtility : IKYUtility
    {
        private readonly Dictionary<GPAction, int> _MapAction;

        public KYUtility()
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

        public int? GetAction(GPAction action)
        {
            return _MapAction.Where(x => x.Key == action).FirstOrDefault().Value;
        }

        public string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null)
        {
            var value = JsonSerializer.Serialize(request, options);
            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(value, options);
            return string.Join("&", items.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }

        public string HashMD5(string data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] dataByte = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] result = md5.ComputeHash(dataByte);
            string ret = "";
            for (int i = 0; i < result.Length; i++)
                ret += result[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        public string AesEncrypt(string data, string key, bool isEncodeUrl = true)
        {
            if (string.IsNullOrEmpty(data)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(data);

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

        public string AesDecrypt(string data, string key, bool isDecodeUrl = true)
        {
            if (string.IsNullOrEmpty(data)) return null;
            if (isDecodeUrl)
                data = System.Web.HttpUtility.UrlDecode(data);

            Byte[] toEncryptArray = Convert.FromBase64String(data);

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
