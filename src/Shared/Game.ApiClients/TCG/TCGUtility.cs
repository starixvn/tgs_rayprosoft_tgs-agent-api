using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models;

namespace TGS.Shared.Game.ApiClients.TCG
{
    public interface ITCGUtility
    {
        /// <summary>
        /// Descs the encrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string DESCEncrypt(string str, string key);

        /// <summary>
        /// Descs the decrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string DESCDecrypt(string str, string key);

        /// <summary>
        /// Gets the hash256.
        /// </summary>
        /// <param name="datas">The datas.</param>
        /// <param name="hash_key">The hash key.</param>
        /// <returns></returns>
        string GetHash256(string datas, string hash_key);

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        string GetPrefix(string prefix);
    }
    public class TCGUtility : ITCGUtility
    {
        string ITCGUtility.DESCEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            using (var dESCrypto = new DESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,

            })
            {

                ICryptoTransform cTransform = dESCrypto.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        string ITCGUtility.DESCDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;


            Byte[] toEncryptArray = Convert.FromBase64String(str);

            using (var dESCrypto = new DESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                ICryptoTransform cTransform = dESCrypto.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }

        string ITCGUtility.GetHash256(string datas, string hash_key)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();

            byte[] inputBytes = Encoding.UTF8.GetBytes(datas + hash_key);
            byte[] outputBytes = sha256.ComputeHash(inputBytes);
            return BitConverter.ToString(outputBytes).Replace("-", "").ToLower();
        }

        string ITCGUtility.GetPrefix(string prefix)
            => $"{Consts.GameProvider.TCG.ToLower()}.{prefix}";
    }
}
