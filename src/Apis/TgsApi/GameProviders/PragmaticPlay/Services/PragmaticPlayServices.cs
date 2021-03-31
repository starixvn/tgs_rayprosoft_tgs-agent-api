using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public interface IPragmaticPlayServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        ValueTask CacheGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalRoundId,
            int gameId, string externalTransactionId);

        ValueTask<string> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, PlatformType platform,
            string externalRoundId, string externalGameId, string externalGameActionGroupId);
    }

    public sealed class PragmaticPlayServices : IPragmaticPlayServices
    {
        private static readonly string ExternalTransactionIdCacheType = "PP:TXID";
        private readonly IRedisCacheManager _cacheManager;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public PragmaticPlayServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IRedisCacheManager cacheManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _cacheManager = cacheManager;
        }

        async ValueTask<GameProviderConfigurationData> IPragmaticPlayServices.GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContext?.Items ??
                throw new InvalidOperationException("Can not get http context items");

            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }

        public async ValueTask CacheGameActionExternalIdAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId, string externalTransactionId)
        {
            var cacheKey = GetExternalTransactionIdCacheKey(gameProviderId, playerCode, externalRoundId, gameId);
            await _cacheManager.SafeCacheAsync(ExternalTransactionIdCacheType, cacheKey, TimeSpan.FromMinutes(5),
                externalTransactionId).ConfigureAwait(false);
        }

        async ValueTask<string> IPragmaticPlayServices.GetGameActionExternalIdAsync(Guid gameProviderId,
            string playerCode, PlatformType platform, string externalRoundId, string externalGameId,
            string externalGameActionGroupId)
        {
            var gamePlatformType = platform.ToGamePlatformTypes();
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId);
            var gameId = games.Where(g => gamePlatformType.Contains(g.PlatformType)).Select(g => g.Id).FirstOrDefault();

            var cacheKey = GetExternalTransactionIdCacheKey(gameProviderId, playerCode, externalRoundId, gameId);
            var externalTransactionId = await _cacheManager.SafeGetAsync(ExternalTransactionIdCacheType, cacheKey,
                () => Task.FromResult(default(string))).ConfigureAwait(false);

            if (externalTransactionId.IsNotNullOrEmpty())
            {
                return externalTransactionId;
            }

            var gameActionExternalIds =
                await _gameActionQueries.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(gameProviderId,
                        playerCode, externalRoundId, gameId, externalGameActionGroupId)
                    .ConfigureAwait(false);

            return gameActionExternalIds.FirstOrDefault();
        }

        private static string GetExternalTransactionIdCacheKey(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId) => $"{gameProviderId}:{playerCode}:{externalRoundId}:{gameId}";
    }
}