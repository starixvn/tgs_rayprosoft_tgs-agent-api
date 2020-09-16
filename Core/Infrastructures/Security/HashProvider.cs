using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.Infrastructures.Security
{
    public interface IHashProvider
    {
        string Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind, Encoding encoding);

        string Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind);

        byte[] Hash(byte[] data, byte[] salt, HashAlgorithmKind algorithm);

        byte[] Hash(byte[] data);

        byte[] GenerateRandomSalt(ushort length);
    }

    public sealed class HashProvider : IHashProvider
    {
        private readonly IHashProvider _hashProvider;
        private readonly ITgsHashConfig _config;
        private readonly RNGCryptoServiceProvider _randomGenerator = new RNGCryptoServiceProvider();

        private static readonly IDictionary<HashAlgorithmKind, Func<HashAlgorithm>> AlgByKind =
            new Dictionary<HashAlgorithmKind, Func<HashAlgorithm>>
            {
                {HashAlgorithmKind.Md5, () => new MD5CryptoServiceProvider()},
                {HashAlgorithmKind.Sha1, () => new SHA1CryptoServiceProvider()},
                {HashAlgorithmKind.Sha256, () => new SHA256Managed()},
                {HashAlgorithmKind.Sha384, () => new SHA384Managed()},
                {HashAlgorithmKind.Sha512, () => new SHA512Managed()}
            };

        public HashProvider(ITgsHashConfig config)
        {
            _hashProvider = this;
            _config = config;
        }

        string IHashProvider.Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind, Encoding encoding)
        {
            if (AlgByKind.TryGetValue(algorithm, out var func))
            {
                var data = encoding.GetBytes(plainText);

                var hash = func().ComputeHash(data);
                switch (kind)
                {
                    case HashStringKind.Base64:
                        return hash.ToBase64String();

                    case HashStringKind.Hex:
                        return hash.ToHexString();
                }
            }
            throw new ArgumentException($"Unknown hash algorithm {algorithm}");
        }

        string IHashProvider.Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind)
        {
            return _hashProvider.Hash(plainText, algorithm, kind, Encoding.UTF8);
        }

        byte[] IHashProvider.Hash(byte[] data, byte[] salt, HashAlgorithmKind algorithm)
        {
            if (AlgByKind.TryGetValue(algorithm, out var func))
            {
                return func().ComputeHash(BinaryUtil.ConcatenateBytes(salt, data));
            }
            throw new ArgumentException($"Unknown hash algorithm {algorithm}");
        }

        /// <summary>
        /// Hash using configuration salt and algorithm
        /// </summary>
        byte[] IHashProvider.Hash(byte[] data)
        {
            return
                _hashProvider.Hash(data, _config.Salt, _config.HashAlgorithm);
        }

        byte[] IHashProvider.GenerateRandomSalt(ushort length)
        {
            if (length == 0) throw new ArgumentException("Length of random bytes generated cannot be 0");

            var bytes = new byte[length];
            _randomGenerator.GetNonZeroBytes(bytes);
            return bytes;
        }
    }
}