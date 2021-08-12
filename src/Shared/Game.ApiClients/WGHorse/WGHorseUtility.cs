using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TGS.Shared.Game.ApiClients.WGHorse
{
    public class WGHorseUtility
    {
        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="secretIV">The secret iv.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string secretKey, string secretIV, bool isEncodeUrl = true)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] IVBytes = Encoding.UTF8.GetBytes(secretIV);
            using (RijndaelManaged cipher = new RijndaelManaged())
            {
                cipher.Mode = CipherMode.CBC;
                cipher.Padding = PaddingMode.PKCS7;
                cipher.KeySize = 128;
                cipher.BlockSize = 128;
                cipher.Key = keyBytes;
                cipher.IV = IVBytes;

                byte[] valueBytes = Encoding.UTF8.GetBytes(str);

                byte[] encrypted;
                using (ICryptoTransform encryptor = cipher.CreateEncryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = ms.ToArray();

                            var base64Str = Convert.ToBase64String(encrypted, 0, encrypted.Length);

                            return isEncodeUrl ? System.Web.HttpUtility.UrlEncode(base64Str) : base64Str;
                        }
                    }
                }
            }
        }
    }
}
