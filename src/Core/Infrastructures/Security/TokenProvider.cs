using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.Core.Infrastructures.Security
{
    public interface ITokenProvider
    {
        Task<string> EncryptAsync<T>(T data);

        Task<T> DecryptAsync<T>(string token);
    }

    public sealed class TokenProvider : ITokenProvider
    {
        private static readonly string _iv = "37F8BF8F8EA48FD2";
        private static readonly string _key = "D42F6412E48DD386";
        private readonly ISymmetricCryptoProvider _cryptoProvider;
        private readonly IJsonSerializationProvider _json;

        private readonly JsonSerializerOptions _options;

        public TokenProvider(
            ISymmetricCryptoProviderFactory cryptoProviderFactory,
            IJsonSerializationProvider json)
        {
            _json = json;
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.AES);

            _options = new() {WriteIndented = false};
        }

        async Task<T> ITokenProvider.DecryptAsync<T>(string token)
        {
            var value = await _cryptoProvider.DecryptStringAsync(token, _iv, _key);
            return _json.DeserializeFromString<T>(value, _options);
        }

        Task<string> ITokenProvider.EncryptAsync<T>(T data)
        {
            var value = _json.SerializeToString(data, _options);
            return _cryptoProvider.EncryptStringAsync(value, _iv, _key);
        }
    }
}