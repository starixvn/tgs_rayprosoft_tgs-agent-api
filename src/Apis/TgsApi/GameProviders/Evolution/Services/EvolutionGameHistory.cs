using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.Evolution;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult;
using TGS.Shared.Game.Models.Evolution.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public sealed class EvolutionGameHistory : BaseBetHistory
    {
        private readonly IEvolutionApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public EvolutionGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            IEvolutionApiClient client,
            ISettingOperations settingOperations,
            IJsonSerializationProvider jsonSerializationProvider
            ) : base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _client = client ?? throw new System.ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new System.ArgumentNullException(nameof(settingOperations));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new System.ArgumentNullException(nameof(jsonSerializationProvider));
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {

            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<EvolutionConfiguration>(SettingConsts.GameProviderConfigurations.Evolution).ConfigureAwait(false);
            var response = await _client.GetGamHistoryRoundAsync(configuration, new EvolutionGamHistoryRoundRequest() { PlayerCode = playerCode, RoundId = round.ExternalRoundId }).ConfigureAwait(false);

            if (cachedGame.Type == GameType.Slot)
            {
                var gameResult = _jsonSerializationProvider.DeserializeFromString<SlotGames>(_jsonSerializationProvider.SerializeToString(response.Data.Result));
                var path = gameResult.Path.Substring(1, gameResult.Path.Length - 1);
                var url = $"{configuration.ApiUrl}{path}";
                return new RoundResult()
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new BaseData[]
                    {
                        new UrlData {Url=url}
                    }
                };
            }
            else
            {
                return new RoundResult
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new BaseData[]
                    {
                        new JsonData { Value = _jsonSerializationProvider.SerializeToString(response.Data.Result) }
                    }
                };
            }
        }
    }
}
