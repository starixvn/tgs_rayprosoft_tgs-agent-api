using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IHabaneroApiClient
    {
        ValueTask<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request);

        ValueTask<FundTransferResponse> FundTransferAsync(FundTransferRequest request);

        ValueTask<QueryResponse> QueryGameActionAsync(QueryRequest queryRequest);
    }

    public sealed class HabaneroApiClient : BaseApiClient, IHabaneroApiClient
    {
        private static readonly string RoutePrefix = "api/provider";

        public HabaneroApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) :
            base(client, logger, json)
        {
        }

        ValueTask<AuthorizePlayerResponse> IHabaneroApiClient.AuthorizePlayerAsync(AuthorizePlayerRequest request) =>
            ExecuteJsonRequestAsync<AuthorizePlayerRequest, AuthorizePlayerResponse>(HttpMethod.Post,
                $"{RoutePrefix}/authorize", request);

        ValueTask<FundTransferResponse> IHabaneroApiClient.FundTransferAsync(FundTransferRequest request) =>
            ExecuteJsonRequestAsync<FundTransferRequest, FundTransferResponse>(HttpMethod.Post,
                $"{RoutePrefix}/transfer", request);

        ValueTask<QueryResponse> IHabaneroApiClient.QueryGameActionAsync(QueryRequest request) =>
            ExecuteJsonRequestAsync<QueryRequest, QueryResponse>(HttpMethod.Post,
                $"{RoutePrefix}/transfer/query", request);
    }
}