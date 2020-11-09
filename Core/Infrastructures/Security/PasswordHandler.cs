using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TGS.Core.Infrastructures.Security
{
    public interface IPasswordHandler
    {
        (byte[], byte[]) GenerateCredential(string password);

        bool Verify(byte[] existedPassword, byte[] existedSalt, string password);
    }

    public sealed class PasswordHandler : IPasswordHandler
    {
        private readonly IHashProvider _hashProvider;
        private readonly RNGCryptoServiceProvider _randomGenerator = new RNGCryptoServiceProvider();

        public PasswordHandler(IHashProvider hashProvider)
        {
            _hashProvider = hashProvider;
        }

        (byte[], byte[]) IPasswordHandler.GenerateCredential(string password)
        {
            var salt = GenerateRandomSalt(32);
            var hashedPassword = _hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                _hashProvider.Hash(salt));

            return (salt, hashedPassword);
        }

        bool IPasswordHandler.Verify(byte[] existedPassword, byte[] existedSalt, string password)
        {
            var salt = _hashProvider.Hash(existedSalt);
            var hashedPassword = _hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                salt);

            return existedPassword.SequenceEqual(hashedPassword);
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
    }
}