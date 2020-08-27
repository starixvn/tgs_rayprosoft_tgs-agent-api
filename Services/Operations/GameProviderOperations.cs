using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IGameProviderOperations
    {
        Task<Guid> GetGameProviderIdByCodeAsync(string gameProviderCode);
    }

    public sealed class GameProviderOperations : IGameProviderOperations
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly Func<IGameProviderQueries> _gameProviderQueriesFactory;

        public GameProviderOperations(
            IRedisCacheManager cacheManager,
            Func<IGameProviderQueries> gameProviderQueriesFactory)
        {
            _cacheManager = cacheManager;
            _gameProviderQueriesFactory = gameProviderQueriesFactory;
        }

        Task<Guid> IGameProviderOperations.GetGameProviderIdByCodeAsync(string gameProviderCode)
        {
            return _cacheManager.SafeGetAndCacheAsync(RedisCacheType.GameProviderIdByCode, gameProviderCode, TimeSpan.FromHours(2),
                () => _gameProviderQueriesFactory().GetGameProviderIdByCodeAsync(gameProviderCode));
        }
    }
}