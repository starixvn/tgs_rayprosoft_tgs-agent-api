using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class CountPlayerFavoriteGameQueryService : IGetService<CountPlayerFavoriteGameQuery, int>
    {
        private readonly IGameProviderRepository _gameProviderRepository;

        public CountPlayerFavoriteGameQueryService(IGameProviderRepository gameProviderRepository)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public ValueTask<int> GetAsync(CountPlayerFavoriteGameQuery query, CancellationToken cancellationToken = default)
            => _gameProviderRepository
                .QueryPlayerFavoriteGamesAsync(query.BrandId, query.PlayerCode)
                .CountAsync(cancellationToken);
    }
}
