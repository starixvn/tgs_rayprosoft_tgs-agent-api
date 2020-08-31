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
                Code = r.Code,
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
            var value = await _cacheManager.GetHashSetAsync(RedisCacheType.GameProviderConfiguration, gameProviderId, gameProviderConfigurationId.ToString());
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
            var value = await _cacheManager.GetHashSetAsync(RedisCacheType.GameProvider, gameProviderId, CachedGameProviderFields.Status);
            if (value.HasValue)
            {
                return (Status)(int)value;
            }

            var cachedGameProvider = await _interface.GetCachedGameProviderAsync(gameProviderId);
            return cachedGameProvider?.Status;
        }

        async Task<bool> IGameProviderOperations.GetGameProviderSupportTestPlayerAsync(Guid gameProviderId)
        {
             var value = await _cacheManager.GetHashSetAsync(RedisCacheType.GameProvider, gameProviderId, CachedGameProviderFields.SupportTestPlayer);
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
                async () => {
                    var dict = await _gameProviderQueriesFactory().GetGameProviderLanguagesByInternalCodeAsync(gameProviderId);
                    return dict.Keys.ToArray();
                });

            return languages.Contains(cultureCode, StringComparer.OrdinalIgnoreCase);
        }


        #region Private Methods

        private async Task<CachedGameProvider> GetCachedGameProviderOrNullAsync(Guid gameProviderId)
        {
            try
            {
                var values = await _cacheManager.GetHashSetAsync(RedisCacheType.GameProvider, gameProviderId, new RedisValue[]
                {
                    CachedGameProviderFields.GameProviderCode,
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
                        Code = values[0],
                        Status = (Status)(int)values[1],
                        Description = values[2],
                        AutoClosedRound = (bool)values[3],
                        SupportTestPlayer = (bool)values[4],
                        PlaceBetTimeout = (int)values[5],
                        SettleBetTimeout = (int)values[6]
                    };
                }
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to get cached player for player code: {playerCode}", e);
            }

            return null;
        }

        private async Task CacheGameProviderAsync(Guid gameProviderId, CachedGameProvider gameProvider)
        {
            try
            {
                var fields = new HashEntry[]
                {
                    new (CachedGameProviderFields.GameProviderCode, gameProvider.Code),
                    new (CachedGameProviderFields.Status, (int)gameProvider.Status),
                    new (CachedGameProviderFields.Decription, gameProvider.Description),
                    new (CachedGameProviderFields.AutoClosedRound, gameProvider.AutoClosedRound),
                    new (CachedGameProviderFields.SupportTestPlayer, gameProvider.SupportTestPlayer),
                    new (CachedGameProviderFields.PlaceBetTimeout, gameProvider.PlaceBetTimeout),
                    new (CachedGameProviderFields.SettleBetTimeout, gameProvider.SettleBetTimeout)
                };

                await _cacheManager.CacheHashSetAsync(RedisCacheType.GameProvider, gameProviderId, fields, TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to cache player for playerId: {playerCode}", e);
            }
        }

        private async Task<CachedGameProviderConfiguration> GetCachedGameProviderConfigurationOrNullAsync(Guid gameProviderId, Guid gameProviderConfigurationId)
        {
            try
            {
                var data = await _cacheManager.GetHashSetAsync(RedisCacheType.GameProviderConfiguration, gameProviderId, gameProviderConfigurationId.ToString());

                if (data.HasValue)
                {
                    return _json.DeserializeFromString<CachedGameProviderConfiguration>(data);
                }
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to get cached player for player code: {playerCode}", e);
            }

            return null;
        }

        private async Task CacheGameProviderConfigurationAsync(Guid gameProviderId, Guid gameProviderConfigurationId, CachedGameProviderConfiguration gpc)
        {
            try
            {
                var fields = new HashEntry[]
                {
                    new(gameProviderConfigurationId.ToString(), _json.SerializeToString(gpc))
                };

                await _cacheManager.CacheHashSetAsync(RedisCacheType.GameProviderConfiguration, gameProviderId, fields, TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to get cached player for player code: {playerCode}", e);
            }
        }

        private static class CachedGameProviderFields
        {
            public const string GameProviderCode = "gameProviderCode";

            public const string Status = "status";

            public const string Decription = "description";

            public const string AutoClosedRound = "autoClosedRound";

            public const string SupportTestPlayer = "supportTestPlayer";

            public const string PlaceBetTimeout = "placeBetTimeout";

            public const string SettleBetTimeout = "settleBetTimeout";
        }

        #endregion
    }
}