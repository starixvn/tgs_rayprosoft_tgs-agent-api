using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.PragmaticPlay.Outgoing;

namespace TGS.Shared.Game.ApiClients.PragmaticPlay
{
    public interface IPragmaticPlayApiClient
    {
        ValueTask<OpenHistoryExtendedResponse> GetRoundHistoryAsync(Uri apiUrl, OpenHistoryExtendedRequest request);
    }

    public sealed class PragmaticPlayApiClient : BaseApiClient, IPragmaticPlayApiClient
    {
        public PragmaticPlayApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async ValueTask<OpenHistoryExtendedResponse> IPragmaticPlayApiClient.
            GetRoundHistoryAsync(Uri apiUrl, OpenHistoryExtendedRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            return await ExecuteFormRequestAsync<OpenHistoryExtendedResponse>(apiUrl,
                    "IntegrationService/v3/http/HistoryAPI/OpenHistoryExtended/", parameters)
                .ConfigureAwait(false);
        }
    }
}