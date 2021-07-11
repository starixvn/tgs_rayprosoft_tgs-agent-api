using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.Extensions;
using System.Text.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Shared.Game.Models.Evolution.Outgoing;
using System.Collections.Generic;
using System.Text;

namespace TGS.Shared.Game.ApiClients.Evolution
{
    public interface IEvolutionApiClient
    {
        ValueTask<EvolutionGameUrlResponse> GetGameUrlAsync(EvolutionConfiguration configuration, EvolutionGameUrlRequest request);

        ValueTask<EvolutionGamHistoryRoundResponse> GetGamHistoryRoundAsync(EvolutionConfiguration configuration, EvolutionGamHistoryRoundRequest request);
    }

    public sealed class EvolutionApiClient : BaseApiClient, IEvolutionApiClient
    {
        private readonly string _gameProvider = Consts.GameProvider.Evolution;

        public EvolutionApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async ValueTask<EvolutionGameUrlResponse> IEvolutionApiClient.GetGameUrlAsync(EvolutionConfiguration configuration, EvolutionGameUrlRequest request)
        {
            return await ExecuteJsonRequestAsync<EvolutionGameUrlRequest, EvolutionGameUrlResponse>(HttpMethod.Post, configuration.Ua2AuthenticationUrl, string.Empty, request, logPrefix: $"{_gameProvider}.GetGameUrl").ConfigureAwait(false);
        }

        async ValueTask<EvolutionGamHistoryRoundResponse> IEvolutionApiClient.GetGamHistoryRoundAsync(EvolutionConfiguration configuration, EvolutionGamHistoryRoundRequest request)
        {
            var url = $"https://{configuration.ApiUrl}/players/{request.PlayerCode}/games/{request.RoundId}";
            var headers = GettHeaders(configuration.CasinoID, configuration.GameHistoryApiToken);
            return await ExecuteJsonRequestAsync<EvolutionGamHistoryRoundRequest, EvolutionGamHistoryRoundResponse>(HttpMethod.Get, url, null, headers: headers, logPrefix: $"{_gameProvider}.GetGamHistoryRound").ConfigureAwait(false);
        }

        private Dictionary<string, string> GettHeaders(string casinoKey, string apiToken) => new Dictionary<string, string> {
             { "Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{casinoKey}:{apiToken}"))}" }
        };

    }
}
