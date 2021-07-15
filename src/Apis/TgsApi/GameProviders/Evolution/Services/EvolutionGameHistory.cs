using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        private readonly IEvolutionServices _services;
        private readonly IEvolutionGameHistoryService _gameHistoryService;

        public EvolutionGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            IEvolutionApiClient client,
            IEvolutionServices services,
            IEvolutionGameHistoryService gameHistoryService
        ) : base(roundQueries, gameProviderOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var configuration = await _services.GetConfigurationAsync().ConfigureAwait(false);
            var response = await _client.GetGamHistoryRoundAsync(configuration,
                new EvolutionGamHistoryRoundRequest
                {
                    PlayerCode = playerCode,
                    RoundId = round.ExternalRoundId
                }).ConfigureAwait(false);

            if (cachedGame.Type == GameType.Slot)
            {
                return new RoundResult
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new BaseData[]
                    {
                        new UrlData {Url = _gameHistoryService.GetGameHistorySlot(configuration,response)}
                    }
                };
            }

            return new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new JsonData {Value = _gameHistoryService.GetGameHistory(response)}
                }
            };
        }
    }
}