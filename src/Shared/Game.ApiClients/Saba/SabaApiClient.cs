using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Json.JsonConverters;
using TGS.Shared.Game.Models.Saba.Outgoing;

namespace TGS.Shared.Game.ApiClients.Saba
{
    public interface ISabaApiClient
    {
        ValueTask<LoginResponse> LoginAsync(Uri apiUrl, LoginRequest loginRequest);

        ValueTask<CreateMemberResponse> CreateMemberAsync(Uri apiUrl, CreateMemberRequest createMemberRequest);
    }

    public sealed class SabaApiClient : BaseApiClient, ISabaApiClient
    {
        public SabaApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        ValueTask<CreateMemberResponse> ISabaApiClient.CreateMemberAsync(Uri apiUrl,
            CreateMemberRequest request)
        {
            var options = new JsonSerializerOptions {Converters = {new NumberToStringConverter()}};

            var value = Json.SerializeToString(request, options);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value, options);

            return ExecuteFormRequestAsync<CreateMemberResponse>(apiUrl,
                "api/CreateMember", parameters);
        }

        ValueTask<LoginResponse> ISabaApiClient.LoginAsync(Uri apiUrl, LoginRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            return ExecuteFormRequestAsync<LoginResponse>(apiUrl,
                "api/LogIn", parameters);
        }
    }
}