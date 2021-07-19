using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.Operator;
using TGS.Services.Operator.GameProviders;

namespace TGS.Operator.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public GameProviderRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository;
        }

        public IAsyncEnumerable<SubGameProviderInfo> QuerySubGameProvidersAsync(IEnumerable<Guid> gameProviderIds)
        {
            var query = _tgsRepository.SubGameProviders.AsAsyncEnumerable()
                .Where(sgp => gameProviderIds.Contains(sgp.GameProviderId))
                .Select(sgp =>
                    new SubGameProviderInfo(
                        sgp.GameProviderId,
                        sgp.Code, sgp.Name,
                        sgp.Description,
                        sgp.Localization));

            return query;
        }

        public IAsyncEnumerable<GameProviderInfo> QueryGameProvidersAsync(IEnumerable<Guid> gameProviderIds,
            IEnumerable<GamePlatformType> gamePlatformTypes)
        {
            var query = from gameProvider in _tgsRepository.GameProviders.AsQueryable()
                        where gameProviderIds.Contains(gameProvider.Id)
                        select new GameProviderInfo(
                            gameProvider.Id,
                            gameProvider.Code,
                            gameProvider.Status,
                            gameProvider.TransferWallet,
                            gameProvider.Games
                                .Where(g => gamePlatformTypes.Contains(g.PlatformType))
                                .Select(g => new GameTypeAndStatusInfo(g.Type, g.Status, g.SubGameProviderCode)),
                            gameProvider.Currencies.Select(c => c.CurrencyCode));

            return query.ToAsyncEnumerable();
        }
    }
}