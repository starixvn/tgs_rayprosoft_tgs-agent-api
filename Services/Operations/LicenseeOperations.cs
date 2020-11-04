using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface ILicenseeOperations
    {
        Task<CachedLicensee> GetCachedLicenseeAsync(Guid licenseeId);

        Task<string> GetLicenseeCodeByLicenseeIdAsync(Guid licenseeId);

        Task<Status?> GetLicenseeStatusAsync(Guid licenseeId);

        Task<CachedLicenseeGameProviderConfiguration> GetLicenseeGameProviderConfigurationAsync(Guid licenseeId,
            Guid gameProviderId);

        Task<Guid> GetLicenseeGameProviderConfigurationIdAsync(Guid licenseeId, Guid gameProviderId);

        Task<CachedLicenseeGame[]> GetLicenseeGamesAsync(Guid licenseeId, PlatformType platformType);

        Task<CachedLicenseeGame[]> GetLicenseeGamesAsync(Guid licenseeId, Guid gameProviderId,
            PlatformType? platformType = null);

        Task<string> GetTimeZoneByLicenseeIdAsync(Guid licenseeId);

        Task<Guid> GetLicenseeIdByClientIdAsync(string clientId);

        Task<string> GetLicenseeSecretKeyAsync(Guid licenseeId);

        Task<CachedLicenseeGameProvider[]> GetLicenseeGameProvidersAsync(Guid licenseeId, string currencyCode, PlatformType platformType);
    }

    public sealed class LicenseeOperations : ILicenseeOperations
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly ILicenseeOperations _interface;
        private readonly IJsonSerializationProvider _json;
        private readonly ILicenseeQueries _licenseeQueries;
        private readonly ITgsLog _log;

        public LicenseeOperations(
            IRedisCacheManager cacheManager,
            ILicenseeQueries licenseeQueriesFactory,
            IJsonSerializationProvider json,
            ITgsLog log)
        {
            _cacheManager = cacheManager;
            _licenseeQueries = licenseeQueriesFactory;
            _json = json;
            _log = log;

            _interface = this;
        }

        async Task<CachedLicensee> ILicenseeOperations.GetCachedLicenseeAsync(Guid licenseeId)
        {
            var cachedLicensee = await GetCachedLicenseeOrNullAsync(licenseeId);
            if (cachedLicensee != null)
            {
                return cachedLicensee;
            }

            var r = await _licenseeQueries.GetCachedLicenseeAsync(licenseeId);
            if (r == null)
            {
                throw new InvalidOperationException($"Licensee not found with id: {licenseeId}");
            }

            cachedLicensee = new ()
            {
                Code = r.Code,
                Name = r.Name,
                TimeZoneId = r.TimeZoneId,
                Status = r.Status,
                ResellerId = r.ResellerId,
                ResellerCode = r.ResellerCode,
                ClientId = r.ClientId,
                SecretKey = r.SecretKey
            };

            await CacheLicenseeAsync(licenseeId, cachedLicensee);

            return cachedLicensee;
        }

        async Task<string> ILicenseeOperations.GetLicenseeCodeByLicenseeIdAsync(Guid licenseeId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId,
                CachedLicenseeFields.LicenseeCode);
            if (value.HasValue)
            {
                return value;
            }

            var cachedLicensee = await _interface.GetCachedLicenseeAsync(licenseeId);
            return cachedLicensee?.Code;
        }

        async Task<Status?> ILicenseeOperations.GetLicenseeStatusAsync(Guid licenseeId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId,
                CachedLicenseeFields.Status);
            if (value.HasValue)
            {
                return (Status) (int) value;
            }

            var cachedLicensee = await _interface.GetCachedLicenseeAsync(licenseeId);
            return cachedLicensee?.Status;
        }

        async Task<Guid> ILicenseeOperations.GetLicenseeGameProviderConfigurationIdAsync(Guid licenseeId,
            Guid gameProviderId)
        {
            var lgpc = await _interface.GetLicenseeGameProviderConfigurationAsync(licenseeId, gameProviderId);
            return lgpc.GameProviderConfigurationId;
        }

        async Task<CachedLicenseeGameProviderConfiguration> ILicenseeOperations.
            GetLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId)
        {
            var lgpc = await GetCachedLicenseeGameProviderConfigurationOrNullAsync(licenseeId, gameProviderId);
            if (lgpc != null)
            {
                return lgpc;
            }

            var r = await _licenseeQueries.GetCachedLicenseeGameProviderConfigurationAsync(licenseeId, gameProviderId);
            if (r == null)
            {
                throw new InvalidOperationException(
                    $"LicenseeGameProviderConfiguration doesn't exist with licenseeId: {licenseeId} and gameProviderId: {gameProviderId}");
            }

            lgpc = new CachedLicenseeGameProviderConfiguration
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = r.GameProviderConfigurationId,
                DefaultLanguageCode = r.DefaultLanguageCode,
                LicenseeLogoUrl = r.LicenseeLogoUrl,
                QueryStrings = new QueryString
                {
                    Desktop = r.DesktopQueryString,
                    Mobile = r.MobileQueryString
                }
            };

            await CacheLicenseeGameProviderConfigurationAsync(licenseeId, gameProviderId, lgpc);

            return lgpc;
        }

        async Task<CachedLicenseeGame[]> ILicenseeOperations.GetLicenseeGamesAsync(Guid licenseeId,
            PlatformType platformType)
        {
            var games = await _cacheManager.SafeGetAndCacheAsync(RedisCacheType.LicenseeGames, licenseeId,
                TimeSpan.FromHours(2),
                () => _licenseeQueries.GetLicenseeGamesAsync(licenseeId));

            return games.Where(g => g.PlatformType == platformType)
                .ToArray();
        }

        async Task<CachedLicenseeGame[]> ILicenseeOperations.GetLicenseeGamesAsync(Guid licenseeId, Guid gameProviderId,
            PlatformType? platformType)
        {
            var games = await _cacheManager.SafeGetAndCacheAsync(RedisCacheType.LicenseeGames, licenseeId,
                TimeSpan.FromHours(2),
                () => _licenseeQueries.GetLicenseeGamesAsync(licenseeId));

            return games.Where(g => g.GameProviderId == gameProviderId &&
                                    (platformType == null || g.PlatformType == platformType))
                .ToArray();
        }

        async Task<string> ILicenseeOperations.GetTimeZoneByLicenseeIdAsync(Guid licenseeId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId,
                CachedLicenseeFields.TimeZoneId);
            if (value.HasValue)
            {
                return value;
            }

            var cachedLicensee = await _interface.GetCachedLicenseeAsync(licenseeId);
            return cachedLicensee?.TimeZoneId;
        }

        Task<Guid> ILicenseeOperations.GetLicenseeIdByClientIdAsync(string clientId)
        {
            return _cacheManager.SafeGetAndCacheAsync(RedisCacheType.Licensee, clientId, TimeSpan.FromHours(2),
                () => _licenseeQueries.GetLicenseeIdByClientIdAsync(clientId));
        }

        async Task<string> ILicenseeOperations.GetLicenseeSecretKeyAsync(Guid licenseeId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId,
                CachedLicenseeFields.SecretKey);
            if (value.HasValue)
            {
                return value;
            }

            var cachedLicensee = await _interface.GetCachedLicenseeAsync(licenseeId);
            return cachedLicensee?.SecretKey;
        }

        async Task<CachedLicenseeGameProvider[]> ILicenseeOperations.GetLicenseeGameProvidersAsync(Guid licenseeId,
            string currencyCode, PlatformType platformType)
        {
            var cacheType = RedisCacheType.LicenseeGameProviders;
            var fieldName =
                $"{licenseeId}:{currencyCode}:{platformType}";
            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, null, fieldName);
            if (value.HasValue)
            {
                return _json.DeserializeFromString<CachedLicenseeGameProvider[]>(value);
            }

            var gameProviders = await
                _licenseeQueries.GetLicenseeGameProvidersAsync(licenseeId, currencyCode, platformType);

            var fields = new HashEntry[]
            {
                new (fieldName, _json.SerializeToString(gameProviders))
            };

            await _cacheManager.SafeCacheHashSetAsync(cacheType, null, fields, TimeSpan.FromHours(2));

            return gameProviders;
        }

        private async Task<CachedLicensee> GetCachedLicenseeOrNullAsync(Guid licenseeId)
        {
            try
            {
                var values = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId,
                    new RedisValue[]
                    {
                        CachedLicenseeFields.LicenseeCode,
                        CachedLicenseeFields.LicenseeName,
                        CachedLicenseeFields.TimeZoneId,
                        CachedLicenseeFields.Status,
                        CachedLicenseeFields.ResellerId,
                        CachedLicenseeFields.ResellerCode,
                        CachedLicenseeFields.ClientId,
                        CachedLicenseeFields.SecretKey
                    });

                if (values.All(a => !a.IsNull))
                {
                    return new CachedLicensee
                    {
                        Code = values[0],
                        Name = values[1],
                        TimeZoneId = values[2],
                        Status = (Status) (int) values[3],
                        ResellerId = Guid.Parse(values[4]),
                        ResellerCode = values[5],
                        ClientId = values[6],
                        SecretKey = values[7]
                    };
                }
            }
            catch (Exception e)
            {
                _log.LogError($"Failed to get cached licensee for licenseeId: {licenseeId}", e);
            }

            return null;
        }

        private async Task CacheLicenseeAsync(Guid licenseeId, CachedLicensee licensee)
        {
            try
            {
                var fields = new HashEntry[]
                {
                    new (CachedLicenseeFields.LicenseeCode, licensee.Code),
                    new (CachedLicenseeFields.LicenseeName, licensee.Name),
                    new (CachedLicenseeFields.TimeZoneId, licensee.TimeZoneId),
                    new (CachedLicenseeFields.Status, (int)licensee.Status),
                    new (CachedLicenseeFields.ResellerId, licensee.ResellerId.ToString()),
                    new (CachedLicenseeFields.ResellerCode, licensee.ResellerCode),
                    new (CachedLicenseeFields.ClientId, licensee.ClientId),
                    new (CachedLicenseeFields.SecretKey, licensee.SecretKey)
                };

                await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.Licensee, licenseeId, fields,
                    TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                _log.LogError($"Failed to cache licensee for licenseeId: {licenseeId}", e);
            }
        }

        private async Task<CachedLicenseeGameProviderConfiguration>
            GetCachedLicenseeGameProviderConfigurationOrNullAsync(Guid licenseeId, Guid gameProviderId)
        {
            try
            {
                var data = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.LicenseeGameProviderConfiguration,
                    licenseeId, gameProviderId.ToString());

                if (data.HasValue)
                {
                    return _json.DeserializeFromString<CachedLicenseeGameProviderConfiguration>(data);
                }
            }
            catch (Exception e)
            {
                _log.LogError($"Failed to get cached licensee game provider configuration for licenseeId: {licenseeId}",
                    e);
            }

            return null;
        }

        private async Task CacheLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId,
            CachedLicenseeGameProviderConfiguration bgpc)
        {
            try
            {
                var fields = new HashEntry[]
                {
                    new (gameProviderId.ToString(), _json.SerializeToString(bgpc))
                };

                await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.LicenseeGameProviderConfiguration, licenseeId,
                    fields, TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                _log.LogError($"Failed to cache licensee game provider configuration for licenseeId: {licenseeId}", e);
            }
        }

        private static class CachedLicenseeFields
        {
            public const string LicenseeCode = "licenseeCode";
            public const string LicenseeName = "licenseeName";
            public const string TimeZoneId = "timezoneId";
            public const string Status = "status";
            public const string ResellerId = "resellerId";
            public const string ResellerCode = "resellerCode";
            public const string ClientId = "clientId";
            public const string SecretKey = "secretKey";
        }
    }
}