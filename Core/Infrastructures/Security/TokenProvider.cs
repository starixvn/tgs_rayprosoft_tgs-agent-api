using System.Text.Json;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.Core.Infrastructures.Security
{
    public interface ITokenProvider
    {
        string Encrypt<T>(T data);

        T Decrypt<T>(string token);
    }

    public sealed class TokenProvider : ITokenProvider
    {
        private static readonly string _iv = "37F8BF8F8EA48FD2";
        private static readonly string _key = "D42F6412E48DD386";
        private readonly IJsonSerializationProvider _json;
        private readonly ISymmetricCryptoProvider _cryptoProvider;

        public TokenProvider(
            ISymmetricCryptoProviderFactory cryptoProviderFactory,
            IJsonSerializationProvider json)
        {
            _json = json;
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.AES);
        }

        T ITokenProvider.Decrypt<T>(string token)
        {
            var value = _cryptoProvider.DecryptString(token, _iv, _key);
            return _json.DeserializeFromString<T>(value);
        }

        string ITokenProvider.Encrypt<T>(T data)
        {
            var value = _json.SerializeToString(data, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            return _cryptoProvider.EncryptString(value, _iv, _key);
        }
    }
}
