// ReSharper disable InconsistentNaming

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TGS.Core.Infrastructures.Base62;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.Infrastructures.Security
{
    internal sealed class AESCryptoProvider : ISymmetricCryptoProvider
    {
        string ISymmetricCryptoProvider.DecryptString(string encrypted, string iv, string key, CipherMode mode,
            PaddingMode padding, Encoding encoding)
        {
            if (encrypted.IsNullOrEmpty())
            {
                return encrypted;
            }

            ValidateIVAndKey(iv, key);

            encoding ??= Encoding.UTF8;

            using var aes = new AesCryptoServiceProvider
            {
                IV = encoding.GetBytes(iv),
                Key = encoding.GetBytes(key),
                Mode = mode,
                Padding = padding
            };

            using var ct = aes.CreateDecryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = encrypted.FromBase62();
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            return encoding.GetString(ms.ToArray());
        }

        string ISymmetricCryptoProvider.EncryptString(string original, string iv, string key, CipherMode mode,
            PaddingMode padding, Encoding encoding)
        {
            if (original.IsNullOrEmpty())
            {
                return original;
            }

            ValidateIVAndKey(iv, key);

            encoding ??= Encoding.UTF8;

            using var aes = new AesCryptoServiceProvider
            {
                IV = encoding.GetBytes(iv),
                Key = encoding.GetBytes(key),
                Mode = mode,
                Padding = padding
            };

            using var ct = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = encoding.GetBytes(original);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            return ms.ToArray().ToBase62();
        }

        private static void ValidateIVAndKey(string iv, string key)
        {
            if (iv.IsNullOrEmpty() || key.IsNullOrEmpty())
            {
                throw new InvalidOperationException("IV and Key for AES should not be null or empty");
            }
        }
    }
}