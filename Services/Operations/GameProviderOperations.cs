using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IGameProviderOperations
    {
        Task<Guid> GetGameProviderIdByCodeAsync(string gameProviderCode);

        Task<CachedGameProvider> GetCachedGameProviderAsync(Guid gameProviderId);

        Task<CachedGameProviderConfiguration> GetCachedGameProviderConfigurationAsync(Guid gameProviderId, Guid gameProviderConfigurationId);

        Task<Status?> GetGameProviderStatusAsync(Guid gameProviderId);

        Task<Status?> GetGameProviderConfigurationStatusAsync(Guid gameProviderId, Guid gameProviderConfigurationId);

        Task<bool> GetGameProviderSupportTestPlayerAsync(Guid gameProviderId);

        Task<bool> LanguageSupportedAsync(Guid gameProviderId, string cultureCode);

        Task<bool> CurrencySupportedAsync(Guid gameProviderId, string currencyCode);

        Task<string> GetGameProviderBetLimitCodeAsync(Guid gameProviderId, string currencyCode, int betLimitGroupId);

        Task<CachedGame[]> GetCachedGamesAsync(Guid gameProviderId);

        Task<CachedGame> GetCachedGameAsync(Guid gameProviderId, Guid gameId);

        Task<GameStatus?> GetGameStatusAsync(Guid gameProviderId, Guid gameId);
    }

    public sealed class GameProviderOperations : IGameProviderOperations
    {
        private readonly IRedisCacheManager _cacheManager;

        private readonly Func<IGameProviderQueries> _gameProviderQueriesFactory;

        private readonly IJsonSerializationProvider _json;

        private readonly IGameProviderOperations _interface;

        public GameProviderOperations(
            IRedisCacheManager cacheManager,
            Func<IGameProviderQueries> gameProviderQueriesFactory,
            IJsonSerializationProvider json)
        {
            _cacheManager = cacheManager;
            _gameProviderQueriesFactory = gameProviderQueriesFactory;
            _json = json;
            _interface = this;
        }

        async Task<CachedGameProvider> IGameProviderOperations.GetCachedGameProviderAsync(Guid gameProviderId)
        {
            var cachedGameProvider = await GetCachedGameProviderOrNullAsync(gameProviderId);
            if (cachedGameProvider != null)
            {
                return cachedGameProvider;
            }

            var r = await _gameProviderQueriesFactory().GetCachedGameProviderAsync(gameProviderId);
            cachedGameProvider = new()
            {
                Id = gameProviderId,
                Code = r.Code,
                Name = r.Name,
                Description = r.Description,
                Status = r.Status,
                AutoClosedRound = r.AutoClosedRound,
                SupportTestPlayer = r.SupportTestPlayer,
                PlaceBetTimeout = r.PlaceBetTimeout,
                SettleBetTimeout = r.SettleBetTimeout
            };

            await CacheGameProviderAsync(gameProviderId, cachedGameProvider);

            return cachedGameProvider;
        }

        async Task<CachedGameProviderConfiguration> IGameProviderOperations.GetCachedGameProviderConfigurationAsync(Guid gameProviderId, Guid gameProviderConfigurationId)
        {
            var gpc = await GetCachedGameProviderConfigurationOrNullAsync(gameProviderId, gameProviderConfigurationId);
            if (gpc != null)
            {
                return gpc;
            }

            var r = await _gameProviderQueriesFactory().GetCachedGameProviderConfigurationAsync(gameProviderConfigurationId);
            if (r == null)
            {
                throw new InvalidOperationException($"GameProviderConfiguration doesn't exist with gameProviderId: {gameProviderId} and gameProviderConfigurationId: {gameProviderConfigurationId}");
            }

            gpc = new()
            {
                Status = r.Status,
                GameProviderId = gameProviderId,
                SecretKey = r.SecretKey,
                DesktopEndpoint = r.DesktopEndpoint,
                MobileEndpoint = r.MobileEndpoint,
                ConfigurationData = r.ConfigurationData,
                IsDefault = r.IsDefault
            };

            await CacheGameProviderConfigurationAsync(gameProviderId, gameProviderConfigurationId, gpc);

            return gpc;
        }

        async Task<Status?> IGameProviderOperations.GetGameProviderConfigurationStatusAsync(Guid gameProviderId, Guid gameProviderConfigurationId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.GameProviderConfiguration, gameProviderId, gameProviderConfigurationId.ToString());
            var gpc = value.HasValue ?
                 _json.DeserializeFromString<CachedGameProviderConfiguration>(value) :
                 await _interface.GetCachedGameProviderConfigurationAsync(gameProviderId, gameProviderConfigurationId);

            return gpc.Status;
        }

        Task<Guid> IGameProviderOperations.GetGameProviderIdByCodeAsync(string gameProviderCode)
        {
            return _cacheManager.SafeGetAndCacheAsync(RedisCacheType.GameProviderIdByCode, gameProviderCode, TimeSpan.FromHours(2),
                () => _gameProviderQueriesFactory().GetGameProviderIdByCodeAsync(gameProviderCode));
        }

        async Task<Status?> IGameProviderOperations.GetGameProviderStatusAsync(Guid gameProviderId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.GameProvider, gameProviderId, CachedGameProviderFields.Status);
            if (value.HasValue)
            {
                return (Status)(int)value;
            }

            var cachedGameProvider = await _interface.GetCachedGameProviderAsync(gameProviderId);
            return cachedGameProvider?.Status;
        }

        async Task<bool> IGameProviderOperations.GetGameProviderSupportTestPlayerAsync(Guid gameProviderId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.GameProvider, gameProviderId, CachedGameProviderFields.SupportTestPlayer);
            if (value.HasValue)
            {
                return (bool)value;
            }

            var cachedGameProvider = await _interface.GetCachedGameProviderAsync(gameProviderId);
            return cachedGameProvider?.SupportTestPlayer ?? false;
        }

        async Task<bool> IGameProviderOperations.LanguageSupportedAsync(Guid gameProviderId, string cultureCode)
        {
            var languages = await _cacheManager.SafeGetAndCacheAsync(RedisCacheType.GameProviderLanguages, gameProviderId, TimeSpan.FromHours(2),
                async () =>
                {
                    var dict = await _gameProviderQueriesFactory().GetGameProviderLanguagesByInternalCodeAsync(gameProviderId);
                    return dict.Keys.ToArray();
                });

            return languages.Contains(cultureCode, StringComparer.OrdinalIgnoreCase);
        }

        async Task<bool> IGameProviderOperations.CurrencySupportedAsync(Guid gameProviderId, string currencyCode)
        {
            var currencies = await _cacheManager.SafeGetAndCacheAsync(RedisCacheType.GameProviderCurrencies, gameProviderId, TimeSpan.FromHours(2),
                async () =>
                {
                    var dict = await _gameProviderQueriesFactory().GetGameProviderCurrenciesByInternalCodeAsync(gameProviderId);
                    return dict.Keys.ToArray();
                });

            return currencies.Contains(currencyCode, StringComparer.OrdinalIgnoreCase);
        }

        async Task<string> IGameProviderOperations.GetGameProviderBetLimitCodeAsync(Guid gameProviderId, string currencyCode, int betLimitGroupId)
        {
            var field = $"{currencyCode}:{betLimitGroupId}";
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.GameProviderBetLimitCode, gameProviderId, field);
            if (value.HasValue)
            {
                return value;
            }

            var betLimitCode = await _gameProviderQueriesFactory().GetGameProviderBetLimitCodeAsync(gameProviderId, currencyCode, betLimitGroupId);

            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.GameProviderBetLimitCode, gameProviderId, new HashEntry[] {
                 new (field, betLimitCode)
                }, TimeSpan.FromHours(2));

            return betLimitCode;
        }

        async Task<CachedGame[]> IGameProviderOperations.GetCachedGamesAsync(Guid gameProviderId)
        {
            var sets = await _cacheManager.SafeGetAllHashSetAsync(RedisCacheType.Games, gameProviderId);
            if (sets.Length > 0)
            {
                return sets.Select(s => _json.DeserializeFromString<CachedGame>(s.Value)).ToArray();
            }

            return await GetAndCacheGamesAsync(gameProviderId);
        }

        async Task<CachedGame> IGameProviderOperations.GetCachedGameAsync(Guid gameProviderId, Guid gameId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Games, gameProviderId, gameId.ToString());
            if (value.HasValue)
            {
                return _json.DeserializeFromString<CachedGame>(value);
            }

            var games = await GetAndCacheGamesAsync(gameProviderId);
            return games.FirstOrDefault(g => g.Id == gameId);
        }

        async Task<GameStatus?> IGameProviderOperations.GetGameStatusAsync(Guid gameProviderId, Guid gameId)
        {
            var game = await _interface.GetCachedGameAsync(gameProviderId, gameId);
            return game?.Status;
        }

        #region Private Methods

        private async Task<CachedGameProvider> GetCachedGameProviderOrNullAsync(Guid gameProviderId)
        {
            var values = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.GameProvider, gameProviderId, new RedisValue[]
            {
                CachedGameProviderFields.GameProviderId,       
                CachedGameProviderFields.GameProviderCode,
                CachedGameProviderFields.GameProviderName,
                CachedGameProviderFields.Decription,
                CachedGameProviderFields.Status,
                CachedGameProviderFields.AutoClosedRound,
                CachedGameProviderFields.SupportTestPlayer,
                CachedGameProviderFields.PlaceBetTimeout,
                CachedGameProviderFields.SettleBetTimeout
            });

            if (values.All(a => !a.IsNull))
            {
                return new CachedGameProvider
                {
                    Id = Guid.Parse(values[0]),
                    Code = values[1],
                    Name = values[2],
                    Status = (Status)(int)values[3],
                    Description = values[4],
                    AutoClosedRound = (bool)values[5],
                    SupportTestPlayer = (bool)values[6],
                    PlaceBetTimeout = (int)values[7],
                    SettleBetTimeout = (int)values[8]
                };
            }

            return null;
        }

        private async Task CacheGameProviderAsync(Guid gameProviderId, CachedGameProvider gameProvider)
        {
            var fields = new HashEntry[]
            {
                new (CachedGameProviderFields.GameProviderId, gameProvider.Id.ToString()),
                new (CachedGameProviderFields.GameProviderCode, gameProvider.Code),
                new (CachedGameProviderFields.GameProviderName, gameProvider.Name),
                new (CachedGameProviderFields.Status, (int)gameProvider.Status),
                new (CachedGameProviderFields.Decription, gameProvider.Description),
                new (CachedGameProviderFields.AutoClosedRound, gameProvider.AutoClosedRound),
                new (CachedGameProviderFields.SupportTestPlayer, gameProvider.SupportTestPlayer),
                new (CachedGameProviderFields.PlaceBetTimeout, gameProvider.PlaceBetTimeout),
                new (CachedGameProviderFields.SettleBetTimeout, gameProvider.SettleBetTimeout)
            };

            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.GameProvider, gameProviderId, fields, TimeSpan.FromHours(2), true);
        }

        private async Task<CachedGameProviderConfiguration> GetCachedGameProviderConfigurationOrNullAsync(Guid gameProviderId, Guid gameProviderConfigurationId)
        {
            var data = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.GameProviderConfiguration, gameProviderId, gameProviderConfigurationId.ToString());

            if (data.HasValue)
            {
                return _json.DeserializeFromString<CachedGameProviderConfiguration>(data);
            }

            return null;
        }

        private async Task CacheGameProviderConfigurationAsync(Guid gameProviderId, Guid gameProviderConfigurationId, CachedGameProviderConfiguration gpc)
        {
            var fields = new HashEntry[]
            {
                new(gameProviderConfigurationId.ToString(), _json.SerializeToString(gpc))
            };

            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.GameProviderConfiguration, gameProviderId, fields, TimeSpan.FromHours(2), true);
        }

        private async Task<CachedGame[]> GetAndCacheGamesAsync(Guid gameProviderId)
        {
            var games = await _gameProviderQueriesFactory().GetCachedGamesAsync(gameProviderId);

            var fields = games.Select(g => new HashEntry(g.Id.ToString(), _json.SerializeToString(g)))
                              .ToArray();

            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.Games, gameProviderId, fields, TimeSpan.FromHours(2));
            return games;
        }

        private static class CachedGameProviderFields
        {
            public const string GameProviderId = "gameProviderId";
            public const string GameProviderCode = "gameProviderCode";
            public const string GameProviderName = "gameProviderName";
            public const string Status = "status";
            public const string Decription = "description";
            public const string AutoClosedRound = "autoClosedRound";
            public const string SupportTestPlayer = "supportTestPlayer";
            public const string PlaceBetTimeout = "placeBetTimeout";
            public const string SettleBetTimeout = "settleBetTimeout";
        }

        #endregion Private Methods
    }
}