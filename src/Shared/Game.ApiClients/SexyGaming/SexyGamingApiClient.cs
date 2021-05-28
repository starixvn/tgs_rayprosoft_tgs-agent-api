using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;

namespace TGS.Shared.Game.ApiClients.SexyGaming
{
    public interface ISexyGamingApiClient
    {

        Task<SexyGamingCreateMemberResponse> CreatePlayerAsync(SexyGamingConfiguration configuration, SexyGamingCreateMemberRequest request);

        Task<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingGameUrlRequest request);
    }

    public sealed class SexyGamingApiClient : BaseApiClient, ISexyGamingApiClient
    {

        private readonly string _gameProvider = Consts.GameProvider.SexyGaming;

        public SexyGamingApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
           : base(client, logger, json)
        {
        }


        public async Task<SexyGamingCreateMemberResponse> CreatePlayerAsync(SexyGamingConfiguration configuration, SexyGamingCreateMemberRequest request)
        {
            return await GetAPIAsync<SexyGamingCreateMemberRequest, SexyGamingCreateMemberResponse>(configuration, "/wallet/createMember", request, $"{_gameProvider}.CreatePlayerAsync").ConfigureAwait(false);
        }

        public async Task<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingGameUrlRequest request)
        {
            return await GetAPIAsync<SexyGamingGameUrlRequest, SexyGamingGameUrlResponse>(configuration, "/wallet/login", request, $"{_gameProvider}.GetGameUrl").ConfigureAwait(false);
        }

        private ValueTask<TResult> GetAPIAsync<TSource, TResult>(SexyGamingConfiguration configuration, string path, TSource source, string prefix)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(source);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
            return ExecuteFormRequestAsync<TResult>(configuration.ApiUrl, path, data, logPrefix: prefix);

        }
    }
}
