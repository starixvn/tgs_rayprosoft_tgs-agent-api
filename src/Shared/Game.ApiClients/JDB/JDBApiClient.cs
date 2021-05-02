// unset

using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Shared.Game.ApiClients.JDB
{
    public interface IJDBApiClient
    {
        ValueTask<GetTokenResponse> GetTokenAsync(Uri apiUrl, string clientDomain, GetTokenRequest request);
        ValueTask<BetHistoryResponse> GetBetHistoryUrlAsync(Uri apiUrl, string clientDomain, BetHistoryRequest request);
    }
    
    public class JDBApiClient : BaseApiClient, IJDBApiClient
    {
        private readonly ITokenProvider _tokenProvider;
        
        public JDBApiClient(ITokenProvider tokenProvider, HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _tokenProvider = tokenProvider;
        }
        
        #region Game Provider Client Api

        ValueTask<GetTokenResponse> IJDBApiClient.GetTokenAsync(Uri apiUrl, string clientDomain, GetTokenRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.GetToken";
            return ExecuteRequestAsync<GetTokenRequest, GetTokenResponse>(apiUrl, clientDomain, request, prefix);
        }
        
        ValueTask<BetHistoryResponse> IJDBApiClient.GetBetHistoryUrlAsync(Uri apiUrl, string clientDomain, BetHistoryRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.BetHistory";
            return ExecuteRequestAsync<BetHistoryRequest, BetHistoryResponse>(apiUrl, clientDomain, request, prefix);
        }

        private async ValueTask<TResponse> ExecuteRequestAsync<TRequest, TResponse>(Uri apiUrl, string clientDomain, TRequest data, string logPrefix)
        {
            var dataString = await _tokenProvider.EncryptAsync(data);
            var url = $"{apiUrl}/apiRequest.do"
                .AppendQueryString($"dc={clientDomain}")
                .AppendQueryString($"x={dataString}");
            return await ExecuteFormRequestAsync<TResponse>(url, null, logPrefix: logPrefix);
        }

        #endregion
    }
}