using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Services.BetHistory
{
    public abstract class BaseBetHistory : IGameHistory
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IRoundQueries _roundQueries;

        protected BaseBetHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations)
        {
            _roundQueries = roundQueries;
            _gameProviderOperations = gameProviderOperations;
        }

        async ValueTask<GetRoundHistoryResponse> IGameHistory.GetRoundHistoryAsync(GetRoundHistoryRequest request)
        {
            var playerCode = request.PlayerCode;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode)
                .ConfigureAwait(false);

            var round = await _roundQueries.GetRoundByRoundIdAsync(request.RoundId, true, true)
                .ConfigureAwait(false);

            if (round == null || round.GameProviderId != gameProviderId || round.PlayerCode != playerCode)
            {
                throw new InvalidGameActionException($"Round with id {request.RoundId} is not found.",
                    InvalidGameActionErrorCode.RoundNotFound);
            }

            var language = request.Language;

            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId)
                .ConfigureAwait(false);

            var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, round.GameId)
                .ConfigureAwait(false);

            var result =
                await GetGameProviderBetHistoryAsync(playerCode, round, cachedGameProvider, cachedGame, language);

            return new()
            {
                GameProviderName = cachedGameProvider.Name,
                GameName = cachedGame.Name,
                BetAmount = result.BetAmount,
                Winloss = result.Winloss,
                Turnover = result.Turnover,
                ExternalRoundId = round.ExternalRoundId,
                GameResult = result.GameResult,
                Data = result.Data
            };
        }

        protected abstract ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language);
    }
}