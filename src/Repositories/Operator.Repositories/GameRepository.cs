using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.Operator;
using TGS.Services.Operator.Games;

namespace TGS.Operator.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public GameRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository;
        }

        public IAsyncEnumerable<GameTypeDataInfo> QueryGameTypeInfosAsync(IEnumerable<Guid> gameProviderIds,
            IEnumerable<GameStatus> statuses, IEnumerable<GamePlatformType> gamePlatformTypes)
        {
            var query = _tgsRepository.Games.AsQueryable()
                .Where(g => gameProviderIds.Contains(g.GameProviderId) &&
                    statuses.Contains(g.Status) &&
                    gamePlatformTypes.Contains(g.PlatformType))
                .Select(g => new GameTypeDataInfo(g.GameProviderId, g.Type, g.SubGameProviderCode))
                .Distinct();

            return query.AsAsyncEnumerable();
        }
    }
}