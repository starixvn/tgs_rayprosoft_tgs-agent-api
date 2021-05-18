// unset

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Shared.Game.ApiClients.JDB
{
    public interface IJDBApiClient
    {
        ValueTask<GetTokenResponse> GetTokenAsync(Uri apiUrl, string clientDomain, string aesIV, string aesKey, GetTokenRequest request);
        ValueTask<BetHistoryResponse> GetBetHistoryUrlAsync(Uri apiUrl, string clientDomain, string aesIV, string aesKey, BetHistoryRequest request);
    }
    
    public class JDBApiClient : BaseApiClient, IJDBApiClient
    {
        private readonly ISymmetricCryptoProvider _cryptoProvider;
        
        public JDBApiClient(
            ISymmetricCryptoProviderFactory cryptoProviderFactory, 
            HttpClient client, 
            IApiClientLogger logger, 
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.AES);
        }
        
        #region Game Provider Client Api

        ValueTask<GetTokenResponse> IJDBApiClient.GetTokenAsync(Uri apiUrl, string clientDomain, string aesIV, string aesKey, GetTokenRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.GetToken";
            return ExecuteRequestAsync<GetTokenRequest, GetTokenResponse>(apiUrl, clientDomain, aesIV, aesKey, request, prefix);
        }
        
        ValueTask<BetHistoryResponse> IJDBApiClient.GetBetHistoryUrlAsync(Uri apiUrl, string clientDomain, string aesIV, string aesKey, BetHistoryRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.BetHistory";
            return ExecuteRequestAsync<BetHistoryRequest, BetHistoryResponse>(apiUrl, clientDomain, aesIV, aesKey, request, prefix);
        }

        private async ValueTask<TResponse> ExecuteRequestAsync<TRequest, TResponse>(Uri apiUrl, string clientDomain, string aesIV, string aesKey, TRequest data, string logPrefix)
        {
            var jsonString = Json.SerializeToString(data);
            var encryptedString = await _cryptoProvider.EncryptStringAsync(jsonString, aesIV, aesKey, encoding: Encoding.UTF8);
            var url = $"{apiUrl}/apiRequest.do"
                .AppendQueryString($"dc={clientDomain}")
                .AppendQueryString($"x={encryptedString}");
            return await ExecuteFormRequestAsync<TResponse>(url, null, logPrefix: logPrefix);
        }

        #endregion
    }
}