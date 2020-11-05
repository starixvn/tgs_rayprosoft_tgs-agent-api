// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Net.Http;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;

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
        public SBOBETApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        Task<GetBalanceResponse> ISBOBETApiClient.GetPlayerBalanceAsync(GetBalanceRequest request) =>
            ExecuteJsonRequestAsync<GetBalanceRequest, GetBalanceResponse>(HttpMethod.Post, "api/GetBalance", request);

        Task<PlaceBetResponse> ISBOBETApiClient.PlaceBetAsync(PlaceBetRequest request) =>
            ExecuteJsonRequestAsync<PlaceBetRequest, PlaceBetResponse>(HttpMethod.Post, "api/Deduct", request);

        Task<SettleBetResponse> ISBOBETApiClient.SettleBetAsync(SettleBetRequest request) =>
            ExecuteJsonRequestAsync<SettleBetRequest, SettleBetResponse>(HttpMethod.Post, "api/Settle", request);

        Task<CancelBetResponse> ISBOBETApiClient.CancelBetAsync(CancelBetRequest request) =>
            ExecuteJsonRequestAsync<CancelBetRequest, CancelBetResponse>(HttpMethod.Post, "api/Cancel", request);

        Task<RollbackResponse> ISBOBETApiClient.RollbackAsync(RollbackRequest request) =>
            ExecuteJsonRequestAsync<RollbackRequest, RollbackResponse>(HttpMethod.Post, "api/Rollback", request);

        Task<GetBetStatusResponse> ISBOBETApiClient.GetBetStatusAsync(GetBetStatusRequest request) =>
            ExecuteJsonRequestAsync<GetBetStatusRequest, GetBetStatusResponse>(HttpMethod.Post, "api/GetBetStatus",
                request);
    }
}