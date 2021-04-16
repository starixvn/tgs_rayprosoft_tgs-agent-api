using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;

namespace TGS.Tests.ApiClients.Operators
{
    public interface IPlayerApiClient
    {
        ValueTask<LoginResponse> PlayerLoginAsync(LoginRequest request, IReadOnlyDictionary<string, string> headers);

        ValueTask<CreatePlayerResponse> CreatePlayerAsync(CreatePlayerRequest request,
            IReadOnlyDictionary<string, string> headers);

        ValueTask<AuthResponse> GetBrandTokenAsync(AuthRequest request);
    }

    public sealed class PlayerApiClient : BaseApiClient, IPlayerApiClient
    {
        public PlayerApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        ValueTask<AuthResponse> IPlayerApiClient.GetBrandTokenAsync(AuthRequest request) =>
            ExecuteJsonRequestAsync<AuthRequest, AuthResponse>(HttpMethod.Post, "api/auth/jwt", request);

        ValueTask<CreatePlayerResponse> IPlayerApiClient.CreatePlayerAsync(CreatePlayerRequest request,
            IReadOnlyDictionary<string, string> headers) =>
            ExecuteJsonRequestAsync<CreatePlayerRequest, CreatePlayerResponse>(HttpMethod.Post, "api/player/create",
                request, headers);

        ValueTask<LoginResponse> IPlayerApiClient.PlayerLoginAsync(LoginRequest request,
            IReadOnlyDictionary<string, string> headers) =>
            ExecuteJsonRequestAsync<LoginRequest, LoginResponse>(HttpMethod.Post, "api/player/login", request, headers);
    }
}