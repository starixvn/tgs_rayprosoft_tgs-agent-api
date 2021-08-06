using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class ListFavoriteGameIdsQueryService : IQueryService<ListFavoriteGameIdsQuery, FavoriteGameInfo>
    {
        private readonly IGameProviderRepository _gameProviderRepository;

        public ListFavoriteGameIdsQueryService(IGameProviderRepository gameProviderRepository)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public IAsyncEnumerable<FavoriteGameInfo> QueryAsync(
            ListFavoriteGameIdsQuery query,
            CancellationToken cancellationToken = default)
            => _gameProviderRepository.QueryPlayerFavoriteGamesAsync(query.BrandId, query.PlayerCode)
                .Where(fg => fg.GameType == query.GameType);
    }
}
