using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public sealed class SexyGamingGameHistory : BaseBetHistory
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISexyGamingGameHistoryService _gameHistoryService;

        public SexyGamingGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ISexyGamingGameHistoryService gameHistoryService
        ) : base(roundQueries, gameProviderOperations)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var gameRecord = JsonSerializer.Deserialize<SexyGamingGameRecord>(round.RoundDetail.AdditionalData);
            var value = default(string);
            if (gameRecord != null)
            {
                var games = await _gameProviderOperations.GetCachedGamesAsync(round.GameProviderId)
                    .ConfigureAwait(false);
                var externalGameId = games.Where(x => x.Id == round.GameId).First().ExternalId;
                value = _gameHistoryService.GetGameHistory(externalGameId, gameRecord);
            }

            return new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new JsonData {Value = value}
                }
            };
        }
    }
}