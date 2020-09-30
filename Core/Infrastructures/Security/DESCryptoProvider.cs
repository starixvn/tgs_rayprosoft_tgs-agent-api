using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.Infrastructures.Security
{
    public interface IDESCryptoProvider
    {
        string EncryptString(string original, string iv, string key, Encoding encoding = null);

        string DecryptString(string encrypted, string iv, string key, Encoding encoding = null);
    }

    public sealed class DESCryptoProvider : IDESCryptoProvider
    {        
        string IDESCryptoProvider.DecryptString(string encrypted, string iv, string key, Encoding encoding)
        {
            if (encrypted.IsNullOrEmpty())
            { 
                return encrypted;
            }

            ValidateIVAndKey(iv, key);

            encoding ??= Encoding.UTF8;

            using var des = new DESCryptoServiceProvider
            {
                IV = encoding.GetBytes(iv),
                Key = encoding.GetBytes(key)
            };

            using var ct = des.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = Convert.FromBase64String(encrypted);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            return encoding.GetString(ms.ToArray());
        }

        string IDESCryptoProvider.EncryptString(string original, string iv, string key, Encoding encoding)
        {
            if (original.IsNullOrEmpty())
            {
                return  original;
            }

            ValidateIVAndKey(iv, key);

            encoding ??= Encoding.UTF8;

            using var des = new DESCryptoServiceProvider
            {
                IV = encoding.GetBytes(iv),
                Key = encoding.GetBytes(key)
            };

            using var ct = des.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = encoding.GetBytes(original);
            cs.Write(bytes, 0 , bytes.Length);
            cs.FlushFinalBlock();
            
            return Convert.ToBase64String(ms.ToArray());
        }

        private static void ValidateIVAndKey(string iv, string key)
        {
            if (iv.IsNullOrEmpty() || key.IsNullOrEmpty())
            {
                throw new InvalidOperationException("IV and Key for DES should not be null or empty");
            }

            if (iv.Length != 8 || key.Length != 8)
            {
                throw new InvalidOperationException("IV and Key for DES should be 8 characters");
            }
        }
    }
}