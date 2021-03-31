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

        ValueTask CacheRoundStatusAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);

        ValueTask<RoundStatus> GetRoundStatusAsync(Guid gameProviderId, string playerCode, PlatformType platformType,
            string externalRoundId, string externalGameId);
    }

    public sealed class PragmaticPlayServices : IPragmaticPlayServices
    {
        private static readonly string RoundCachePrefix = "PP:Round";
        private static readonly string ExternalTransactionIdCacheType = $"{RoundCachePrefix}:TXID";
        private static readonly string RoundStatusCacheType = $"{RoundCachePrefix}:Stautus";

        private readonly IRedisCacheManager _cacheManager;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly IRoundQueries _roundQueries;

        public PragmaticPlayServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IRoundQueries roundQueries,
            IRedisCacheManager cacheManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _roundQueries = roundQueries;
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

        async ValueTask<string> IPragmaticPlayServices.GetGameActionExternalIdAsync(Guid gameProviderId,
            string playerCode, PlatformType platformType, string externalRoundId, string externalGameId,
            string externalGameActionGroupId)
        {
            var gamePlatformType = platformType.ToGamePlatformTypes();
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId);
            var gameId = games.Where(g => gamePlatformType.Contains(g.PlatformType)).Select(g => g.Id).FirstOrDefault();

            var cacheKey = GetCachedKeyByRound(gameProviderId, playerCode, externalRoundId, gameId);
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

        async ValueTask IPragmaticPlayServices.CacheGameActionExternalIdAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId, string externalTransactionId)
        {
            var cacheKey = GetCachedKeyByRound(gameProviderId, playerCode, externalRoundId, gameId);
            await _cacheManager.SafeCacheAsync(ExternalTransactionIdCacheType, cacheKey, TimeSpan.FromMinutes(5),
                externalTransactionId).ConfigureAwait(false);
        }

        async ValueTask IPragmaticPlayServices.CacheRoundStatusAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId)
        {
            var cacheKey = GetCachedKeyByRound(gameProviderId, playerCode, externalRoundId, gameId);
            await _cacheManager.SafeCacheAsync(RoundStatusCacheType, cacheKey, TimeSpan.FromMinutes(5),
                (RoundStatus?)RoundStatus.Closed).ConfigureAwait(false);
        }

        async ValueTask<RoundStatus> IPragmaticPlayServices.GetRoundStatusAsync(Guid gameProviderId, string playerCode,
            PlatformType platformType, string externalRoundId, string externalGameId)
        {
            var gamePlatformType = platformType.ToGamePlatformTypes();
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId);
            var gameId = games.Where(g => gamePlatformType.Contains(g.PlatformType)).Select(g => g.Id).FirstOrDefault();

            var cacheKey = GetCachedKeyByRound(gameProviderId, playerCode, externalRoundId, gameId);
            var roundStatus = await _cacheManager.SafeGetAsync(RoundStatusCacheType, cacheKey,
                    async () => await _roundQueries.GetRoundStatusAsync(gameProviderId, playerCode,
                        externalRoundId, gameId))
                .ConfigureAwait(false);

            return roundStatus ?? RoundStatus.Open;
        }

        private static string GetCachedKeyByRound(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId) => $"{gameProviderId}:{playerCode}:{externalRoundId}:{gameId}";
    }
}