// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Json.JsonConverters;
using TGS.Core.Models;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using TGS.Shared.Game.Models.SBOBET.Outgoing;

namespace TGS.Shared.Game.ApiClients.SBOBET
{
    public interface ISBOApiClient
    {
        #region Internal Api

        ValueTask<BatchResponse> CancelTransactionsAsync(Uri apiUrl, BatchRequest request);

        #endregion

        #region Game Provider Client Api

        ValueTask<LoginResponse> LoginAsync(Uri apiUrl, LoginRequest request);

        ValueTask<LogoutResponse> LogoutAsync(Uri apiUrl, LogoutRequest request);

        ValueTask<RegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request);

        ValueTask<GetRoundHistoryResponse> GetRoundHistoryAsync(Uri apiUrl, GetRoundHistoryRequest request);

        ValueTask<GetBetHistoryResponse> GetBetHistoryAsync(Uri apiUrl, GetBetHistoryRequest request);

        ValueTask<UpdatePlayerBetLimitsResponse>
            UpdatePlayerBetLimitsAsync(Uri apiUrl, UpdatePlayerBetLimitsRequest request);

        #endregion
    }

    public sealed class SBOApiClient : BaseApiClient, ISBOApiClient
    {
        public SBOApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        #region Internal Api

        ValueTask<BatchResponse> ISBOApiClient.CancelTransactionsAsync(Uri apiUrl, BatchRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.BatchCancellation";
            return ExecuteJsonRequestAsync<BatchRequest, BatchResponse>(HttpMethod.Post, apiUrl,
                "/api/sbo/batch/transactions/cancel", request, logPrefix: prefix);
        }

        #endregion

        #region Game Provider Client Api

        ValueTask<LoginResponse> ISBOApiClient.LoginAsync(Uri apiUrl, LoginRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.Login";
            return ExecuteJsonRequestAsync<LoginRequest, LoginResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/login.aspx", request, logPrefix: prefix);
        }

        ValueTask<LogoutResponse> ISBOApiClient.LogoutAsync(Uri apiUrl, LogoutRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.Logout";
            return ExecuteJsonRequestAsync<LogoutRequest, LogoutResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/logout.aspx", request, logPrefix: prefix);
        }

        ValueTask<RegisterPlayerResponse> ISBOApiClient.RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.RegisterPlayer";
            return ExecuteJsonRequestAsync<RegisterPlayerRequest, RegisterPlayerResponse>(
                HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/register-player.aspx",
                request, logPrefix: prefix);
        }

        ValueTask<GetRoundHistoryResponse> ISBOApiClient.GetRoundHistoryAsync(Uri apiUrl,
            GetRoundHistoryRequest request)
        {
            var options = new JsonSerializerOptions {Converters = {new NumberToStringConverter()}};

            var prefix = $"{Consts.GameProvider.SBOBET}.RoundHistory";
            return ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(
                HttpMethod.Post, apiUrl, "/web-root/restricted/report/get-bet-list-by-refnos.aspx",
                request, logPrefix: prefix, options: options);
        }

        ValueTask<GetBetHistoryResponse> ISBOApiClient.GetBetHistoryAsync(Uri apiUrl, GetBetHistoryRequest request)
        {
            var options = new JsonSerializerOptions {Converters = {new NumberToStringConverter()}};

            var prefix = $"{Consts.GameProvider.SBOBET}.BetHistory";
            return ExecuteJsonRequestAsync<GetBetHistoryRequest, GetBetHistoryResponse>(
                HttpMethod.Post, apiUrl, "/web-root/restricted/report/v2/get-bet-list-by-modify-date.aspx",
                request, logPrefix: prefix, options: options);
        }

        ValueTask<UpdatePlayerBetLimitsResponse> ISBOApiClient.UpdatePlayerBetLimitsAsync(Uri apiUrl,
            UpdatePlayerBetLimitsRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.UpdatePlayerBetLimit";
            return ExecuteJsonRequestAsync<UpdatePlayerBetLimitsRequest, UpdatePlayerBetLimitsResponse>(
                HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/update-player-bet-setting-by-sportid-and-markettype.aspx",
                request, logPrefix: prefix);
        }

        #endregion
    }
}