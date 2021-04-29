using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Services.PluggableServices.Models;

namespace TGS.Services.PluggableServices
{
    public interface IPluggalbeGameProviderApiClient
    {
        ValueTask<GetGameUrlResponse> GetGameUrlAsync(Uri apiUrl, GetGameUrlRequest request);

        ValueTask<GetRoundHistoryResponse> GetGameProviderRoundHistory(Uri apiUrl, GetRoundHistoryRequest request);

        ValueTask<GameProviderWalletGetBalanceResponse> GetBalanceAsync(Uri apiUrl, GameProviderWalletGetBalanceRequest request);
    }

    public sealed class PluggalbeGameProviderApiClient : BaseApiClient, IPluggalbeGameProviderApiClient
    {
        public PluggalbeGameProviderApiClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        ValueTask<GetGameUrlResponse> IPluggalbeGameProviderApiClient.GetGameUrlAsync(Uri apiUrl,
            GetGameUrlRequest request)
        {
            var prefix = $"Pluggable.{request.GameProviderCode}.GetGameUrl";
            return ExecuteJsonRequestAsync<GetGameUrlRequest, GetGameUrlResponse>(HttpMethod.Post, apiUrl,
                "/api/pluggable/gameurl", request, logPrefix: prefix);
        }



        ValueTask<GetRoundHistoryResponse> IPluggalbeGameProviderApiClient.GetGameProviderRoundHistory(Uri apiUrl,
            GetRoundHistoryRequest request)
        {
            var prefix = $"Pluggable.{request.GameProviderCode}.GetRoundHistory";
            return ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(HttpMethod.Post, apiUrl,
                "/api/pluggable/history/round", request, logPrefix: prefix);
        }

        ValueTask<GameProviderWalletGetBalanceResponse> IPluggalbeGameProviderApiClient.GetBalanceAsync(Uri apiUrl, GameProviderWalletGetBalanceRequest request)
        {
            var prefix = $"Pluggable.{request.GameProviderCode}.GetBalance";
            return ExecuteJsonRequestAsync<GameProviderWalletGetBalanceRequest, GameProviderWalletGetBalanceResponse>(HttpMethod.Post, apiUrl,
                "/api/pluggable/player/blaance", request, logPrefix: prefix);
        }
    }
}