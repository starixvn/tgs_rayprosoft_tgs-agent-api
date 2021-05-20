// ReSharper disable InconsistentNaming

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.Infrastructures.Security
{
    internal sealed class AESCryptoProvider : ISymmetricCryptoProvider
    {
        async Task<string> ISymmetricCryptoProvider.DecryptStringAsync(string encrypted, string iv, string key,
            CipherMode mode,
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
            await using var ms = new MemoryStream();
            await using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = Convert.FromBase64String(encrypted);
            await cs.WriteAsync(bytes.AsMemory(0, bytes.Length));
            await cs.FlushFinalBlockAsync();

            return encoding.GetString(ms.ToArray());
        }

        async Task<string> ISymmetricCryptoProvider.EncryptStringAsync(string original, string iv, string key,
            CipherMode mode, PaddingMode padding, Encoding encoding)
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
            await using var ms = new MemoryStream();
            await using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var bytes = encoding.GetBytes(original);
            await cs.WriteAsync(bytes.AsMemory(0, bytes.Length));
            await cs.FlushFinalBlockAsync();

            return Convert.ToBase64String(ms.ToArray());
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