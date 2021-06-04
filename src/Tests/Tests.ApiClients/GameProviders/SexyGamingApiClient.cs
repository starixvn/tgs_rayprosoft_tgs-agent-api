using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.SexyGaming.Common;
using TGS.Shared.Game.Models.SexyGaming.Incoming;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface ISexyGamingApiClient
    {
        ValueTask<SexyGamingGetBalanceResponse> GetPlayerBalanceAsync(string key, string token, SexyGamingGetBalanceRequest request);
        ValueTask<SexyGamingGetBalanceResponse> PlaceBetAsync(string key, string token, SexyGamingSingleWalletTxnsRequest<SexyGamingBet> request);
        ValueTask<SexyGamingBaseResponse> SettleBetAsync(string key, SexyGamingSingleWalletTxnsRequest<SexyGamingSettle> request);
    }

    public sealed class SexyGamingApiClient : BaseApiClient, ISexyGamingApiClient
    {
        private static readonly string RoutePrefix = "api/sg";
        private readonly HttpClient _client;
        private readonly IJsonSerializationProvider _json;
        public SexyGamingApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) :
            base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _json = json ?? throw new ArgumentNullException(nameof(json));
        }

        async ValueTask<SexyGamingGetBalanceResponse> ISexyGamingApiClient.GetPlayerBalanceAsync(string key, string token, SexyGamingGetBalanceRequest request)
        {
            var data = new SexyGamingSingleWalletRequest()
            {
                Key = key,
                Message = Json.SerializeToString(request),
                Extension1 = _json.SerializeToString(new { token })
            };
            var parameters = ConvertToKeyValue(data);
            return await ExecuteFormRequestAsync<SexyGamingGetBalanceResponse>($"{RoutePrefix}/entrance", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<SexyGamingGetBalanceResponse> ISexyGamingApiClient.PlaceBetAsync(string key, string token, SexyGamingSingleWalletTxnsRequest<SexyGamingBet> request)
        {
            var data = new SexyGamingSingleWalletRequest()
            {
                Key = key,
                Message = Json.SerializeToString(request),
                Extension1 = _json.SerializeToString(new { token })
            };
            var parameters = ConvertToKeyValue(data);
            return await ExecuteFormRequestAsync<SexyGamingGetBalanceResponse>($"{RoutePrefix}/entrance", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<SexyGamingBaseResponse> ISexyGamingApiClient.SettleBetAsync(string key, SexyGamingSingleWalletTxnsRequest<SexyGamingSettle> request)
        {
            var data = new SexyGamingSingleWalletRequest()
            {
                Key = key,
                Message = Json.SerializeToString(request)
            };
            var parameters = ConvertToKeyValue(data);
            return await ExecuteFormRequestAsync<SexyGamingGetBalanceResponse>($"{RoutePrefix}/entrance", parameters)
                .ConfigureAwait(false);
        }




        private Dictionary<string, string> ConvertToKeyValue(object request)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var value = Json.SerializeToString(request, request.GetType(), options);
            var items = Json.DeserializeFromString<Dictionary<string, object>>(value, options);
            return items.ToDictionary(p => p.Key, p => p.Value?.ToString() ?? string.Empty);
        }
    }
}
