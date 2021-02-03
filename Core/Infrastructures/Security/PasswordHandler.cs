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
        byte[] GenerateRandomSalt(ushort length);
    }

    public sealed class PasswordHandler : IPasswordHandler
    {
        private readonly IHashProvider _hashProvider;
        private readonly IPasswordHandler _interface;
        private readonly RNGCryptoServiceProvider _randomGenerator = new();

        public PasswordHandler(IHashProvider hashProvider)
        {
            _hashProvider = hashProvider;
            _interface = this;
        }

        (byte[], byte[]) IPasswordHandler.GenerateCredential(string password)
        {
            var salt = _interface.GenerateRandomSalt(32);
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

        byte[] IPasswordHandler.GenerateRandomSalt(ushort length)
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