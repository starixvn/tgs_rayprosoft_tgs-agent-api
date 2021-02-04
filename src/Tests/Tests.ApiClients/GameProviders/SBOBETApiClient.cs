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
        Task<GetBalanceResponse> GetPlayerBalanceAsync(GetBalanceRequest request);
        Task<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request);
        Task<SettleBetResponse> SettleBetAsync(SettleBetRequest request);
        Task<CancelBetResponse> CancelBetAsync(CancelBetRequest request);
        Task<RollbackResponse> RollbackAsync(RollbackRequest request);
        Task<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request);
    }

    public sealed class SBOBETApiClient : BaseApiClient, ISBOBETApiClient
    {
        private const string RoutePrefix = "api/provider";

        public SBOBETApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        Task<GetBalanceResponse> ISBOBETApiClient.GetPlayerBalanceAsync(GetBalanceRequest request) =>
            ExecuteJsonRequestAsync<GetBalanceRequest, GetBalanceResponse>(HttpMethod.Post, $"{RoutePrefix}/GetBalance", request);

        Task<PlaceBetResponse> ISBOBETApiClient.PlaceBetAsync(PlaceBetRequest request) =>
            ExecuteJsonRequestAsync<PlaceBetRequest, PlaceBetResponse>(HttpMethod.Post, $"{RoutePrefix}/Deduct", request);

        Task<SettleBetResponse> ISBOBETApiClient.SettleBetAsync(SettleBetRequest request) =>
            ExecuteJsonRequestAsync<SettleBetRequest, SettleBetResponse>(HttpMethod.Post, $"{RoutePrefix}/Settle", request);

        Task<CancelBetResponse> ISBOBETApiClient.CancelBetAsync(CancelBetRequest request) =>
            ExecuteJsonRequestAsync<CancelBetRequest, CancelBetResponse>(HttpMethod.Post, $"{RoutePrefix}/Cancel", request);

        Task<RollbackResponse> ISBOBETApiClient.RollbackAsync(RollbackRequest request) =>
            ExecuteJsonRequestAsync<RollbackRequest, RollbackResponse>(HttpMethod.Post, $"{RoutePrefix}/Rollback", request);

        Task<GetBetStatusResponse> ISBOBETApiClient.GetBetStatusAsync(GetBetStatusRequest request) =>
            ExecuteJsonRequestAsync<GetBetStatusRequest, GetBetStatusResponse>(HttpMethod.Post, $"{RoutePrefix}/GetBetStatus",
                request);
    }
}