using System;
using System.Threading.Tasks;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public class EvolutionExternalTransactionService
    {
        private readonly IEvolutionService _evolutionService;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IEvolutionCacheService _redisCacheService;

        public EvolutionExternalTransactionService(
            IEvolutionCacheService redisCacheService,
            IGameActionQueries gameActionQueries,
            IEvolutionService evolutionService
            )
        {
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _evolutionService = evolutionService ?? throw new ArgumentNullException(nameof(evolutionService));
        }

        public async Task<bool> ExistExternalTransactionAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
        {
            if (await _redisCacheService.ExistExternalTransactionIdAsync(playerCode, externalTransactionId).ConfigureAwait(false))
            {
                return true;
            }
            if (await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId)
                .ConfigureAwait(false) != null)
            {
                return true;
            }
            await _redisCacheService.AddExternalTransactionIdAsync(playerCode, externalTransactionId).ConfigureAwait(false);
            return false;
        }

        public async Task<bool> ExistExternalTransactionCancelAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
        {
            var orderData = await _gameActionQueries.GetGameActionExternalIdAsync(
                gameProviderId,
                playerCode,
                externalTransactionId).ConfigureAwait(false);

            var (RoundClosed, ExternalTransactionCancelId) = _evolutionService.GetCancelExternalTransactionId(externalTransactionId, orderData.GameActionType);

            if (await _redisCacheService.ExistExternalTransactionIdAsync(playerCode, ExternalTransactionCancelId).ConfigureAwait(false))
            {
                return true;
            }
            if (await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, ExternalTransactionCancelId)
                .ConfigureAwait(false) != null)
            {
                return true;
            }
            await _redisCacheService.AddExternalTransactionIdAsync(playerCode, ExternalTransactionCancelId).ConfigureAwait(false);
            return false;
        }
    }
}
