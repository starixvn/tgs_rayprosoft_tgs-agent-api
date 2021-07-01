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

namespace TGS.Shared.Game.ApiClients.Evolution
{
    public interface IEvolutionApiClient
    {
        ValueTask<EvolutionGameUrlResponse> GetGameUrlAsync(EvolutionConfiguration configuration, EvolutionGameUrlRequest request);
    }

    public sealed class EvolutionApiClient : BaseApiClient, IEvolutionApiClient
    {
        private readonly string _gameProvider = Consts.GameProvider.Evolution;

        public EvolutionApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        ValueTask<EvolutionGameUrlResponse> IEvolutionApiClient.GetGameUrlAsync(EvolutionConfiguration configuration, EvolutionGameUrlRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
