using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Json.JsonConverters;
using TGS.Core.Models;
using TGS.Shared.Game.Models.Saba.Outgoing;

namespace TGS.Shared.Game.ApiClients.Saba
{
    public interface ISabaApiClient
    {
        ValueTask<GameUrlResponse> GetGameUrlAsync(Uri apiUrl, GameUrlRequest request);

        ValueTask<CreateMemberResponse> CreateMemberAsync(Uri apiUrl, CreateMemberRequest request);

        ValueTask<GetRoundHistoryResponse> GetBetDetailByTransIdAsync(Uri apiUrl, GetRoundHistoryRequest request);
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

            var prefix = $"{Consts.GameProvider.Saba}.CreateMember";
            return ExecuteFormRequestAsync<CreateMemberResponse>(apiUrl,
                "api/CreateMember", parameters, logPrefix: prefix);
        }

        ValueTask<GameUrlResponse> ISabaApiClient.GetGameUrlAsync(Uri apiUrl, GameUrlRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.Saba}.GetGameUrl";
            return ExecuteFormRequestAsync<GameUrlResponse>(apiUrl,
                "api/GetSabaUrl", parameters, logPrefix: prefix);
        }

        ValueTask<GetRoundHistoryResponse> ISabaApiClient.GetBetDetailByTransIdAsync(Uri apiUrl,
            GetRoundHistoryRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.Saba}.RoundHistory";
            return ExecuteFormRequestAsync<GetRoundHistoryResponse>(apiUrl,
                "api/GetBetDetailByTransID", parameters, logPrefix: prefix);
        }
    }
}