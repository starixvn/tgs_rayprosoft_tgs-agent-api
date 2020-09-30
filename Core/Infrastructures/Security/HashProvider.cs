using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.Core.Infrastructures.Security
{
    public interface IHashProvider
    {
        string Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind, Encoding encoding);

        string Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind);

        (byte[], byte[]) GenerateCredential(string password);

        bool PasswordMatched(byte[] existedPassword, byte[] existedSalt, string password);
    }

    public sealed class HashProvider : IHashProvider
    {
        private readonly IHashProvider _hashProvider;
        private readonly ITgsHashConfig _config;
        private readonly RNGCryptoServiceProvider _randomGenerator = new RNGCryptoServiceProvider();

        private static readonly IDictionary<HashAlgorithmKind, Func<HashAlgorithm>> AlgByKind =
            new Dictionary<HashAlgorithmKind, Func<HashAlgorithm>>
            {
                { HashAlgorithmKind.Md5, () => new MD5CryptoServiceProvider() },
                { HashAlgorithmKind.Sha1, () => new SHA1CryptoServiceProvider() },
                { HashAlgorithmKind.Sha256, () => new SHA256Managed() },
                { HashAlgorithmKind.Sha384, () => new SHA384Managed() },
                { HashAlgorithmKind.Sha512, () => new SHA512Managed() }
            };

        public HashProvider(ITgsHashConfig config)
        {
            _hashProvider = this;
            _config = config;
        }

        (byte[], byte[]) IHashProvider.GenerateCredential(string password)
        {
            var salt = GenerateRandomSalt(32);
            var hashedPassword = Hash(
                Encoding.UTF8.GetBytes(password),
                Hash(salt),
                _config.HashAlgorithm);
            return (salt, hashedPassword);
        }

        bool IHashProvider.PasswordMatched(byte[] existedPassword, byte[]existedSalt, string password)
        {
            var salt = Hash(existedSalt);

            var hashedPassword = Hash(
                Encoding.UTF8.GetBytes(password),
                salt,
                _config.HashAlgorithm);

            return existedPassword.SequenceEqual(hashedPassword);
        }

        string IHashProvider.Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind, Encoding encoding)
        {
            if (AlgByKind.TryGetValue(algorithm, out var func))
            {
                var data = encoding.GetBytes(plainText);

                var hash = func().ComputeHash(data);
                return kind switch
                {
                    HashStringKind.Base64 => hash.ToBase64String(),
                    _ => hash.ToHexString()
                };
            }

            throw new ArgumentException($"Unknown hash algorithm {algorithm}");
        }

        string IHashProvider.Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind)
        {
            return _hashProvider.Hash(plainText, algorithm, kind, Encoding.UTF8);
        }

        /// <summary>
        /// Hash using configuration salt and algorithm
        /// </summary>
        private byte[] Hash(byte[] data)
        {
            return Hash(data, _config.Salt, _config.HashAlgorithm);
        }

        private byte[] GenerateRandomSalt(ushort length)
        {
            if (length == 0)
            {
                throw new ArgumentException("Length of random bytes generated cannot be 0");
            }

            var bytes = new byte[length];
            _randomGenerator.GetNonZeroBytes(bytes);
            return bytes;
        }

        private static byte[] Hash(byte[] data, byte[] salt, HashAlgorithmKind algorithm)
        {
            if (AlgByKind.TryGetValue(algorithm, out var func))
            {
                return func().ComputeHash(BinaryUtil.ConcatenateBytes(salt, data));
            }

            throw new ArgumentException($"Unknown hash algorithm {algorithm}");
        }

    }
}