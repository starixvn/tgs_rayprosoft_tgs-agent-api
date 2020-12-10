// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Clients
{
    public interface ISBOApiClient : IApiClient
    {
        Task<LoginResponse> LoginAsync(Uri apiUrl, LoginRequest request);
        Task<LogoutResponse> LogoutAsync(Uri apiUrl, LogoutRequest request);
        Task<RegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request);
        Task<GetRoundHistoryResponse> GetRoundHistoryAsync(Uri apiUrl, GetRoundHistoryRequest request);
        Task<UpdatePlayerBetLimitsResponse> UpdatePlayerBetLimitsAsync(Uri apiUrl, UpdatePlayerBetLimitsRequest request);
    }

    public sealed class SBOApiClient : BaseApiClient, ISBOApiClient
    {
        public SBOApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        Task<LoginResponse> ISBOApiClient.LoginAsync(Uri apiUrl, LoginRequest request) =>
            ExecuteJsonRequestAsync<LoginRequest, LoginResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/login.aspx", request);

        Task<LogoutResponse> ISBOApiClient.LogoutAsync(Uri apiUrl, LogoutRequest request) =>
            ExecuteJsonRequestAsync<LogoutRequest, LogoutResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/logout.aspx", request);

        Task<RegisterPlayerResponse> ISBOApiClient.RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request) =>
            ExecuteJsonRequestAsync<RegisterPlayerRequest, RegisterPlayerResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/register-player.aspx", request);

        Task<GetRoundHistoryResponse> ISBOApiClient.GetRoundHistoryAsync(Uri apiUrl, GetRoundHistoryRequest request) =>
            ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/report/get-bet-list-by-refnos.aspx", request);

        Task<UpdatePlayerBetLimitsResponse> ISBOApiClient.UpdatePlayerBetLimitsAsync(Uri apiUrl,
            UpdatePlayerBetLimitsRequest request) =>
            ExecuteJsonRequestAsync<UpdatePlayerBetLimitsRequest, UpdatePlayerBetLimitsResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/update-player-bet-setting-by-sportid-and-markettype.aspx", request);
    }
}