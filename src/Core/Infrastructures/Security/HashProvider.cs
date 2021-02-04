using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.IO;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.Core.Infrastructures.Security
{
    public interface IHashProvider
    {
        string Hash(string plainText, HashAlgorithmKind algorithm = HashAlgorithmKind.Sha512, HashStringKind kind = HashStringKind.Hex, Encoding encoding = null);

        byte[] Hash(byte[] data, byte[] salt, HashAlgorithmKind algorithm = HashAlgorithmKind.Sha512);

        byte[] Hash(byte[] data);
    }

    public sealed class HashProvider : IHashProvider
    {
        private static readonly IDictionary<HashAlgorithmKind, Func<HashAlgorithm>> AlgByKind =
            new Dictionary<HashAlgorithmKind, Func<HashAlgorithm>>
            {
                {HashAlgorithmKind.Md5, () => new MD5CryptoServiceProvider()},
                {HashAlgorithmKind.Sha1, () => new SHA1CryptoServiceProvider()},
                {HashAlgorithmKind.Sha256, () => new SHA256Managed()},
                {HashAlgorithmKind.Sha384, () => new SHA384Managed()},
                {HashAlgorithmKind.Sha512, () => new SHA512Managed()}
            };

        private static readonly byte[] Salt =
            BinaryUtil.HexStringToBytes("6AE11BCBF9169EEE6B6A936B16E2B7F0613192927F1E678F7D18D3653FC3CE11");

        private readonly IHashProvider _hashProvider;

        public HashProvider()
        {
            _hashProvider = this;
        }

        string IHashProvider.Hash(string plainText, HashAlgorithmKind algorithm, HashStringKind kind, Encoding encoding)
        {
            encoding ??= Encoding.UTF8;

            if (!AlgByKind.TryGetValue(algorithm, out var func))
            {
                throw new ArgumentException($"Unknown hash algorithm {algorithm}");
            }

            var data = encoding.GetBytes(plainText);

            var hash = func().ComputeHash(data);

            return kind switch
            {
                HashStringKind.Base64 => hash.ToBase64String(),
                _                     => hash.ToHexString()
            };
        }

        byte[] IHashProvider.Hash(byte[] data, byte[] salt, HashAlgorithmKind algorithm) =>
            AlgByKind.TryGetValue(algorithm, out var func)
                ? func().ComputeHash(BinaryUtil.ConcatenateBytes(salt, data))
                : throw new ArgumentException($"Unknown hash algorithm {algorithm}");

        /// <summary>
        /// Hash using default salt and algorithm
        /// </summary>
        byte[] IHashProvider.Hash(byte[] data) => _hashProvider.Hash(data, Salt);
    }
}