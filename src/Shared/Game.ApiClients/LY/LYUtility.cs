using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TGS.Shared.Game.Models.LY.Enums;

namespace TGS.Shared.Game.ApiClients.LY
{
    internal class LYUtility : ILYUtility
    {
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

        public int? GetAction(LYAction action)
        {
            return LYActionMapper.Mapper.Where(x => x.Key == action).FirstOrDefault().Value;
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
    }
}
