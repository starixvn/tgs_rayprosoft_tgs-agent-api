using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Apis.OperatorApi.Models.Messages.Players;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;

namespace TGS.Tests.ApiClients.Operators
{
    public interface IPlayerApiClient
    {
        Task<LoginResponse> PlayerLoginAsync(LoginRequest request, IReadOnlyDictionary<string, string> headers);
    }

    public sealed class PlayerApiClient : BaseApiClient, IPlayerApiClient
    {
        public PlayerApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) 
            :base(client, logger, json)
        {
            
        }

        Task<LoginResponse> IPlayerApiClient.PlayerLoginAsync(LoginRequest request, IReadOnlyDictionary<string, string> headers) => 
            ExecuteJsonRequestAsync<LoginRequest, LoginResponse>(HttpMethod.Post, "api/player/login", request, headers);
    }
}