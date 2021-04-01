using System;
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

        ValueTask<BalanceResponse> GetBalanceAsync(GetBalanceRequest request);

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

        ValueTask<EndRoundResponse> IPragmaticPlayApiClient.EndRoundAsync(EndRoundRequest request) =>
            throw new NotImplementedException();

        ValueTask<BalanceResponse> IPragmaticPlayApiClient.GetBalanceAsync(GetBalanceRequest request) =>
            throw new NotImplementedException();

        ValueTask<PlaceBetResponse> IPragmaticPlayApiClient.PlaceBetAsync(PlaceBetRequest request) =>
            throw new NotImplementedException();

        ValueTask<RefundResponse> IPragmaticPlayApiClient.RefundAsync(RefundRequest request) =>
            throw new NotImplementedException();

        ValueTask<ResultResponse> IPragmaticPlayApiClient.ResultAsync(ResultRequest request) =>
            throw new NotImplementedException();

        private Dictionary<string, string> ConvertToKeyValue(object request)
        {
            var value = Json.SerializeToString(request, request.GetType());
            var items = Json.DeserializeFromString<Dictionary<string, object>>(value);
            return items.ToDictionary(p => p.Key, p => p.Value.ToString());
        }
    }
}