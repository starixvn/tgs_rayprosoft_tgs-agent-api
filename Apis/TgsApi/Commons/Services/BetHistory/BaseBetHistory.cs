using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
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
        private readonly IRoundQueries _roundQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ILocalizationOperations _localizationOperations;

        protected BaseBetHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations)
        {
            _roundQueries = roundQueries;
            _gameProviderOperations = gameProviderOperations;
            _localizationOperations = localizationOperations;
        }

        async Task<GetRoundHistoryResponse> IGameHistory.GetRoundHistoryAsync(GetRoundHistoryRequest request)
        {
            var playerCode = request.PlayerCode;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);
            
            var round = await _roundQueries.GetRoundByRoundIdAsync(request.RoundId, true, true);
            if (round == null || round.GameProviderId != gameProviderId || round.PlayerCode != playerCode)
            {
                throw new InvalidGameActionException($"Round with id {request.RoundId} is not found.",
                    InvalidGameActionErrorCode.RoundNotFound);
            }

            var language = request.Language;

            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);
            var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, round.GameId);

            var result = await GetGameProviderBetHistoryAsync(playerCode, round, cachedGameProvider, cachedGame, language);

            var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gameProviderId, language);

            return new()
            {
                GameProviderName = _localizationOperations.Parse(store, gameProviderId.ToString(), Consts.LocalizationContext.GameProviderName, cachedGameProvider.Name),
                GameName = _localizationOperations.Parse(store, cachedGame.Id.ToString(), Consts.LocalizationContext.GameName, cachedGame.Name),
                ExternalRoundId = round.ExternalRoundId,
                GameResult = result.GameResult,
                Data = result.Data
            };
        }

        protected abstract Task<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language);
    }
}