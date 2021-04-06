using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IPragmaticPlayApiClient
    {
        ValueTask<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request);

        ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);

        ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request);

        ValueTask<ResultResponse> ResultAsync(ResultRequest request);

        ValueTask<EndRoundResponse> EndRoundAsync(EndRoundRequest request);

        ValueTask<RefundResponse> RefundAsync(RefundRequest request);
    }

    public sealed class PragmaticPlayApiClient : BaseApiClient, IPragmaticPlayApiClient
    {
        private static readonly string RoutePrefix = "api/provider";

        public PragmaticPlayApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async ValueTask<AuthorizePlayerResponse> IPragmaticPlayApiClient.
            AuthorizePlayerAsync(AuthorizePlayerRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<AuthorizePlayerResponse>($"{RoutePrefix}/authenticate", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<GetBalanceResponse> IPragmaticPlayApiClient.GetBalanceAsync(GetBalanceRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<GetBalanceResponse>($"{RoutePrefix}/balance", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<PlaceBetResponse> IPragmaticPlayApiClient.PlaceBetAsync(PlaceBetRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<PlaceBetResponse>($"{RoutePrefix}/bet", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<RefundResponse> IPragmaticPlayApiClient.RefundAsync(RefundRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<RefundResponse>($"{RoutePrefix}/refund", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<ResultResponse> IPragmaticPlayApiClient.ResultAsync(ResultRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<ResultResponse>($"{RoutePrefix}/result", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<EndRoundResponse> IPragmaticPlayApiClient.EndRoundAsync(EndRoundRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<EndRoundResponse>($"{RoutePrefix}/endround", parameters)
                .ConfigureAwait(false);
        }

        private Dictionary<string, string> ConvertToKeyValue(object request)
        {
            var value = Json.SerializeToString(request, request.GetType());
            var items = Json.DeserializeFromString<Dictionary<string, object>>(value);
            return items.ToDictionary(p => p.Key, p => p.Value?.ToString() ?? string.Empty);
        }
    }
}