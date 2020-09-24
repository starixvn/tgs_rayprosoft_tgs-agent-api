using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Json;
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

        Task<CachedLicenseeGameProviderConfiguration> GetLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId);

        Task<Guid> GetLicenseeGameProviderConfigurationIdAsync(Guid licenseeId, Guid gameProviderId);

        Task<CachedLicenseeGame[]> GetLicenseeGamesAsync(Guid licenseeId, Guid gameProviderId, PlatformType? platformType = null);

        Task<string> GetTimeZoneByBrandIdAsync(Guid licenseeId);
    }

    public sealed class LicenseeOperations : ILicenseeOperations
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly Func<ILicenseeQueries> _licenseeQueriesFactory;
        private readonly IJsonSerializationProvider _json;
        private readonly ILicenseeOperations _interface;

        public LicenseeOperations(
            IRedisCacheManager cacheManager,
            Func<ILicenseeQueries> licenseeQueriesFactory,
            IJsonSerializationProvider json)
        {
            _cacheManager = cacheManager;
            _licenseeQueriesFactory = licenseeQueriesFactory;
            _json = json;
            _interface = this;
        }

        async Task<CachedLicensee> ILicenseeOperations.GetCachedLicenseeAsync(Guid licenseeId)
        {
            var cachedLicensee = await GetCachedLicenseeOrNullAsync(licenseeId);
            if (cachedLicensee != null)
            {
                return cachedLicensee;
            }

            var r = await _licenseeQueriesFactory().GetCachedLicenseeAsync(licenseeId);
            if (r == null)
            {
                throw new InvalidOperationException($"Licensee not found with id: {licenseeId}");
            }

            cachedLicensee = new()
            {
                Code = r.Code,
                Name = r.Name,
                TimeZoneId = r.TimeZoneId,
                Status = r.Status,
                ResellerId = r.ResellerId,
                ResellerCode = r.ResellerCode
            };

            await CacheLicenseeAsync(licenseeId, cachedLicensee);

            return cachedLicensee;
        }

        async Task<string> ILicenseeOperations.GetLicenseeCodeByLicenseeIdAsync(Guid licenseeId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId, CachedLicenseeFields.LicenseeCode);
            if (value.HasValue)
            {
                return value;
            }

            var cachedLicensee = await _interface.GetCachedLicenseeAsync(licenseeId);
            return cachedLicensee?.Code;
        }

        async Task<Status?> ILicenseeOperations.GetLicenseeStatusAsync(Guid licenseeId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId, CachedLicenseeFields.Status);
            if (value.HasValue)
            {
                return (Status)(int)value;
            }

            var cachedLicensee = await _interface.GetCachedLicenseeAsync(licenseeId);
            return cachedLicensee?.Status;
        }

        async Task<Guid> ILicenseeOperations.GetLicenseeGameProviderConfigurationIdAsync(Guid licenseeId, Guid gameProviderId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.LicenseeGameProviderConfiguration, licenseeId, gameProviderId.ToString());
            var lgpc = value.HasValue ?
                 _json.DeserializeFromString<CachedLicenseeGameProviderConfiguration>(value) :
                 await _interface.GetLicenseeGameProviderConfigurationAsync(licenseeId, gameProviderId);

            return lgpc.GameProviderConfigurationId;
        }
       
        async Task<CachedLicenseeGameProviderConfiguration> ILicenseeOperations.GetLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId)
        {
            var lgpc = await GetCachedLicenseeGameProviderConfigurationOrNullAsync(licenseeId, gameProviderId);
            if (lgpc != null)
            {
                return lgpc;
            }

            var r = await _licenseeQueriesFactory().GetCachedLicenseeGameProviderConfigurationAsync(licenseeId, gameProviderId);
            if (r == null)
            {
                throw new InvalidOperationException($"LicenseeGameProviderConfiguration doesn't exist with licenseeId: {licenseeId} and gameProviderId: {gameProviderId}");
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

        async Task<CachedLicenseeGame[]> ILicenseeOperations.GetLicenseeGamesAsync(Guid licenseeId, Guid gameProviderId, PlatformType? platformType)
        {
            var games = await _cacheManager.SafeGetAndCacheAsync(RedisCacheType.LicenseeGames, licenseeId, TimeSpan.FromHours(2),
                () => _licenseeQueriesFactory().GetLicenseeGamesAsync(licenseeId));

            return games.Where(g => g.GameProviderId == gameProviderId &&
                                    (platformType == null || g.PlatformType == platformType))
                             .ToArray();
        }

        async Task<string> ILicenseeOperations.GetTimeZoneByBrandIdAsync(Guid licenseeId)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId, CachedLicenseeFields.TimeZoneId);
            if (value.HasValue)
            {
                return value;
            }

            var cachedLicensee = await _interface.GetCachedLicenseeAsync(licenseeId);
            return cachedLicensee?.TimeZoneId;
        }

        private async Task<CachedLicensee> GetCachedLicenseeOrNullAsync(Guid licenseeId)
        {
            try
            {
                var values = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Licensee, licenseeId, new RedisValue[]
                {
                    CachedLicenseeFields.LicenseeCode,
                    CachedLicenseeFields.LicenseeName,
                    CachedLicenseeFields.TimeZoneId,
                    CachedLicenseeFields.Status,
                    CachedLicenseeFields.ResellerId,
                    CachedLicenseeFields.ResellerCode
                });

                if (values.All(a => !a.IsNull))
                {
                    return new CachedLicensee
                    {
                        Code = values[0],
                        Name = values[1],
                        TimeZoneId = values[2],
                        Status = (Status)(int)values[3],
                        ResellerId = Guid.Parse(values[4]),
                        ResellerCode = values[5]
                    };
                }
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to get cached player for player code: {playerCode}", e);
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
                    new (CachedLicenseeFields.ResellerCode, licensee.ResellerCode)
                };

                await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.Licensee, licenseeId, fields, TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to cache player for playerId: {playerCode}", e);
            }
        }

        private async Task<CachedLicenseeGameProviderConfiguration> GetCachedLicenseeGameProviderConfigurationOrNullAsync(Guid licenseeId, Guid gameProviderId)
        {
            try
            {
                var data = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.LicenseeGameProviderConfiguration, licenseeId, gameProviderId.ToString());

                if (data.HasValue)
                {
                    return _json.DeserializeFromString<CachedLicenseeGameProviderConfiguration>(data);
                }
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to get cached player for player code: {playerCode}", e);
            }

            return null;
        }

        private async Task CacheLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId, CachedLicenseeGameProviderConfiguration bgpc)
        {
            try
            {
                var fields = new HashEntry[]
                {
                    new(gameProviderId.ToString(), _json.SerializeToString(bgpc))
                };

                await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.LicenseeGameProviderConfiguration, licenseeId, fields, TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to cache player for playerId: {playerCode}", e);
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
        }
    }
}