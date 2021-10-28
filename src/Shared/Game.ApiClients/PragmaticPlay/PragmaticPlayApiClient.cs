using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Shared.Game.Models.PragmaticPlay.Outgoing;

namespace TGS.Shared.Game.ApiClients.PragmaticPlay
{
    public interface IPragmaticPlayApiClient
    {
        ValueTask<GameUrlResponse> GetGameUrlAsync(Uri apiUrl, GameUrlRequest request);

        ValueTask<OpenHistoryExtendedResponse> GetRoundHistoryAsync(Uri apiUrl, OpenHistoryExtendedRequest request);
    }

    public sealed class PragmaticPlayApiClient : BaseApiClient, IPragmaticPlayApiClient
    {
        public PragmaticPlayApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        public async ValueTask<GameUrlResponse> GetGameUrlAsync(Uri apiUrl, GameUrlRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.PragmaticPlay}.GameUrl";

            return await ExecuteFormRequestAsync<GameUrlResponse>(apiUrl,
                    "IntegrationService/v3/http/CasinoGameAPI/game/url",
                    parameters,
                    logPrefix: prefix)
                .ConfigureAwait(false);
        }

        public async ValueTask<OpenHistoryExtendedResponse> GetRoundHistoryAsync(
            Uri apiUrl, OpenHistoryExtendedRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.PragmaticPlay}.RoundHistory";

            return await ExecuteFormRequestAsync<OpenHistoryExtendedResponse>(apiUrl,
                    "IntegrationService/v3/http/HistoryAPI/OpenHistoryExtended/",
                    parameters,
                    logPrefix: prefix)
                .ConfigureAwait(false);
        }
    }
}