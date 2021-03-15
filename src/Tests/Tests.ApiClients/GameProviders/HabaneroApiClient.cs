using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IHabaneroApiClient
    {
        Task<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request);
    }

    public sealed class HabaneroApiClient : BaseApiClient, IHabaneroApiClient
    {
        private static readonly string RoutePrefix = "api/provider";

        public HabaneroApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) :
            base(client, logger, json)
        {
        }

        Task<AuthorizePlayerResponse> IHabaneroApiClient.AuthorizePlayerAsync(AuthorizePlayerRequest request) =>
            ExecuteJsonRequestAsync<AuthorizePlayerRequest, AuthorizePlayerResponse>(HttpMethod.Post,
                $"{RoutePrefix}/authorize", request);
    }
}