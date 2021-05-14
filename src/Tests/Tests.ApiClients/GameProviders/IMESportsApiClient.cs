// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.IMESports.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IIMESportsApiClient
    {
        ValueTask<GetBalanceResponse> GetPlayerBalanceAsync(GetBalanceRequest request);
        ValueTask<BetResponse> PlaceBetAsync(BetRequest request);
        ValueTask<BetResponse> SettleAndCancelBetAsync(BetRequest request);
    }

    public sealed class IMESportsApiClient : BaseApiClient, IIMESportsApiClient
    {
        private static readonly string RoutePrefix = "api/imesports";

        public IMESportsApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        ValueTask<GetBalanceResponse> IIMESportsApiClient.GetPlayerBalanceAsync(GetBalanceRequest request) =>
            ExecuteJsonRequestAsync<GetBalanceRequest, GetBalanceResponse>(HttpMethod.Post, $"{RoutePrefix}/GetBalance",
                request);

        ValueTask<BetResponse> IIMESportsApiClient.PlaceBetAsync(BetRequest request) =>
            ExecuteJsonRequestAsync<BetRequest, BetResponse>(HttpMethod.Post, $"{RoutePrefix}/PlaceBet",
                request);

        ValueTask<BetResponse> IIMESportsApiClient.SettleAndCancelBetAsync(BetRequest request) =>
            ExecuteJsonRequestAsync<BetRequest, BetResponse>(HttpMethod.Post, $"{RoutePrefix}/SettleBet",
                request);
    }
}