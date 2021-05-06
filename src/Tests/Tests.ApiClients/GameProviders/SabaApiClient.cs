using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface ISabaApiClient
    {
        ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);
    }

    public sealed class SabaApiClient : BaseApiClient, ISabaApiClient
    {
        private static readonly string RoutePrefix = "api/saba";

        public SabaApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) :
            base(client, logger, json)
        {
        }

        public ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request) =>
            ExecuteJsonRequestAsync<GetBalanceRequest, GetBalanceResponse>(HttpMethod.Post,
                $"{RoutePrefix}/getbalance", request);
    }
}