// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface ISBOBETApiClient
    {
        ValueTask<GetBalanceResponse> GetPlayerBalanceAsync(GetBalanceRequest request);
        ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request);
        ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request);
        ValueTask<CancelBetResponse> CancelBetAsync(CancelBetRequest request);
        ValueTask<RollbackResponse> RollbackAsync(RollbackRequest request);
        ValueTask<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request);
        ValueTask<BatchResponse> CancelTransactionsAsync(BatchRequest request);
    }

    public sealed class SBOBETApiClient : BaseApiClient, ISBOBETApiClient
    {
        private static readonly string RoutePrefix = "api/sbo";

        public SBOBETApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        ValueTask<GetBalanceResponse> ISBOBETApiClient.GetPlayerBalanceAsync(GetBalanceRequest request) =>
            ExecuteJsonRequestAsync<GetBalanceRequest, GetBalanceResponse>(HttpMethod.Post, $"{RoutePrefix}/GetBalance",
                request);

        ValueTask<PlaceBetResponse> ISBOBETApiClient.PlaceBetAsync(PlaceBetRequest request) =>
            ExecuteJsonRequestAsync<PlaceBetRequest, PlaceBetResponse>(HttpMethod.Post, $"{RoutePrefix}/Deduct",
                request);

        ValueTask<SettleBetResponse> ISBOBETApiClient.SettleBetAsync(SettleBetRequest request) =>
            ExecuteJsonRequestAsync<SettleBetRequest, SettleBetResponse>(HttpMethod.Post, $"{RoutePrefix}/Settle",
                request);

        ValueTask<CancelBetResponse> ISBOBETApiClient.CancelBetAsync(CancelBetRequest request) =>
            ExecuteJsonRequestAsync<CancelBetRequest, CancelBetResponse>(HttpMethod.Post, $"{RoutePrefix}/Cancel",
                request);

        ValueTask<RollbackResponse> ISBOBETApiClient.RollbackAsync(RollbackRequest request) =>
            ExecuteJsonRequestAsync<RollbackRequest, RollbackResponse>(HttpMethod.Post, $"{RoutePrefix}/Rollback",
                request);

        ValueTask<GetBetStatusResponse> ISBOBETApiClient.GetBetStatusAsync(GetBetStatusRequest request) =>
            ExecuteJsonRequestAsync<GetBetStatusRequest, GetBetStatusResponse>(HttpMethod.Post,
                $"{RoutePrefix}/GetBetStatus", request);

        ValueTask<BatchResponse> ISBOBETApiClient.CancelTransactionsAsync(BatchRequest request) =>
            ExecuteJsonRequestAsync<BatchRequest, BatchResponse>(HttpMethod.Post, $"{RoutePrefix}/batch/transactions/cancel",
                request);
    }
}