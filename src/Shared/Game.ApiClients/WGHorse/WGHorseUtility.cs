using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TGS.Shared.Game.ApiClients.WGHorse
{
    public interface IWGHorseUtility
    {
        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="secretIV">The secret iv.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        string AesEncrypt(string str, string secretKey, string secretIV, bool isEncodeUrl = true);

        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="secretIV">The secret iv.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        string AesDecrypt(string data, string secretKey, string secretIV, bool isDecodeUrl = true);
    }

    internal sealed class WGHorseUtility : IWGHorseUtility
    {
        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="secretIV">The secret iv.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        public string AesEncrypt(string data, string secretKey, string secretIV, bool isEncodeUrl = true)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] ivBytes = Encoding.UTF8.GetBytes(secretIV);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            using (RijndaelManaged cipher = new RijndaelManaged())
            {
                cipher.Mode = CipherMode.CBC;
                cipher.Padding = PaddingMode.PKCS7;
                cipher.KeySize = 128;
                cipher.BlockSize = 128;
                cipher.Key = keyBytes;
                cipher.IV = ivBytes;

                byte[] encrypted;
                using (ICryptoTransform encryptor = cipher.CreateEncryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(dataBytes, 0, dataBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = ms.ToArray();

                            var base64Str = Convert.ToBase64String(encrypted, 0, encrypted.Length);

                            return isEncodeUrl ? System.Web.HttpUtility.UrlEncode(base64Str) : base64Str;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="secretIV">The secret iv.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        public string AesDecrypt(string data, string secretKey, string secretIV, bool isDecodeUrl = true)
        {
            if (string.IsNullOrEmpty(data)) return null;
            if (isDecodeUrl)
                data = System.Web.HttpUtility.UrlDecode(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] IVBytes = Encoding.UTF8.GetBytes(secretIV);
            byte[] dataBytes = Convert.FromBase64String(data);

            using (RijndaelManaged cipher = new RijndaelManaged())
            {
                cipher.Mode = CipherMode.CBC;
                cipher.Padding = PaddingMode.PKCS7;
                cipher.KeySize = 128;
                cipher.BlockSize = 128;
                cipher.Key = keyBytes;
                cipher.IV = IVBytes;

                using (ICryptoTransform decryptor = cipher.CreateDecryptor())
                {
                    using (MemoryStream msDecrypt = new MemoryStream(dataBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}