using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IBigGamingApiClient
    {
      
        ValueTask<BigGamingResponse> EntranceAsyncstAsync(BigGamingRequest request);
    }

    public sealed class BigGamingApiClient : BaseApiClient, IBigGamingApiClient
    {
        private static readonly string RoutePrefix = "api/bg";
        private readonly HttpClient _client;

        public BigGamingApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) :
            base(client, logger, json)
        {
            _client = client;
        }

        

        public ValueTask<BigGamingResponse> EntranceAsyncstAsync(BigGamingRequest request) =>
            ExecuteJsonRequestAsync<BigGamingRequest, BigGamingResponse>(HttpMethod.Post,
                $"{RoutePrefix}/entrance", request);
    }
}
