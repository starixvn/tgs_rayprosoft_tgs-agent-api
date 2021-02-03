// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Shared.Game.Models.SBOBET.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Clients
{
    public interface ISBOApiClient
    {
        Task<LoginResponse> LoginAsync(Uri apiUrl, LoginRequest request);
        Task<LogoutResponse> LogoutAsync(Uri apiUrl, LogoutRequest request);
        Task<RegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request);
        Task<GetRoundHistoryResponse> GetRoundHistoryAsync(Uri apiUrl, GetRoundHistoryRequest request);

        Task<UpdatePlayerBetLimitsResponse>
            UpdatePlayerBetLimitsAsync(Uri apiUrl, UpdatePlayerBetLimitsRequest request);
    }

    public sealed class SBOApiClient : BaseApiClient, ISBOApiClient
    {
        public SBOApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        Task<LoginResponse> ISBOApiClient.LoginAsync(Uri apiUrl, LoginRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.Login";
            return ExecuteJsonRequestAsync<LoginRequest, LoginResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/login.aspx", request, logPrefix: prefix);
        }

        Task<LogoutResponse> ISBOApiClient.LogoutAsync(Uri apiUrl, LogoutRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.Logout";
            return ExecuteJsonRequestAsync<LogoutRequest, LogoutResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/logout.aspx", request, logPrefix: prefix);
        }

        Task<RegisterPlayerResponse> ISBOApiClient.RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.RegisterPlayer";
            return ExecuteJsonRequestAsync<RegisterPlayerRequest, RegisterPlayerResponse>(
                HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/register-player.aspx",
                request, logPrefix: prefix);
        }

        Task<GetRoundHistoryResponse> ISBOApiClient.GetRoundHistoryAsync(Uri apiUrl, GetRoundHistoryRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.BetHistory";
            return ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(
                HttpMethod.Post, apiUrl, "/web-root/restricted/report/get-bet-list-by-refnos.aspx",
                request, logPrefix: prefix);
        }

        Task<UpdatePlayerBetLimitsResponse> ISBOApiClient.UpdatePlayerBetLimitsAsync(Uri apiUrl,
            UpdatePlayerBetLimitsRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.UpdatePlayerBetLimit";
            return ExecuteJsonRequestAsync<UpdatePlayerBetLimitsRequest, UpdatePlayerBetLimitsResponse>(
                HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/update-player-bet-setting-by-sportid-and-markettype.aspx",
                request, logPrefix: prefix);
        }
    }
}