using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IPlayerOperations
    {
        Task<CachedPlayer> GetCachedPlayerAsync(string playerCode);

        Task<decimal> GetPlayerBalanceAsync(Guid defaultWalletId, bool dirtyRead);

        Task<CachedPlayerLoginRecord> GetPlayerLatestEnabledLoginRecordAsync(string playerCode, PlatformType? platform);

        Task<CachedPlayerLoginRecord[]> GetPlayerEnabledLoginRecordsAsync(string playerCode);
    }

    public sealed class PlayerOperations : IPlayerOperations
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IJsonSerializationProvider _json;
        private readonly IWalletQueries _walletQueries;
        private readonly Func<IPlayerQueries> _playerQueriesFactory;
        private readonly IPlayerOperations _interface;

        public PlayerOperations(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            IWalletQueries walletQueries,
            Func<IPlayerQueries> playerQueriesFactory)
        {
            _cacheManager = cacheManager;
            _json = json;
            _walletQueries = walletQueries;
            _playerQueriesFactory = playerQueriesFactory;

            _interface = this;
        }

        async Task<CachedPlayer> IPlayerOperations.GetCachedPlayerAsync(string playerCode)
        {
            var cachedPlayer = await GetCachedPlayerOrNullAsync(playerCode);
            if (cachedPlayer != null)
            {
                return cachedPlayer;
            }

            cachedPlayer = await _playerQueriesFactory().GetCachedPlayerAsync(playerCode);

            if (cachedPlayer == null)
            {
                throw new PlayerNotFoundException(playerCode);
            }

            await CachePlayerAsync(playerCode, cachedPlayer);

            return cachedPlayer;
        }

        Task<decimal> IPlayerOperations.GetPlayerBalanceAsync(Guid walletId, bool dirtyRead)
        {
            return _walletQueries.GetBalanceAsync(walletId, dirtyRead);
        }

        async Task<CachedPlayerLoginRecord> IPlayerOperations.GetPlayerLatestEnabledLoginRecordAsync(string playerCode, PlatformType? platform)
        {
            var loginRecords = await _interface.GetPlayerEnabledLoginRecordsAsync(playerCode);

            return loginRecords?.OrderByDescending(r => r.ExpiredOn)
                .First(r => platform == null || r.PlatformType == platform);
        }

        async Task<CachedPlayerLoginRecord[]> IPlayerOperations.GetPlayerEnabledLoginRecordsAsync(string playerCode)
        {
            var field = "enabled";
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.PlayerLoginRecords, playerCode, field);

            return value.HasValue ?
                   _json.DeserializeFromString<CachedPlayerLoginRecord[]>(value) :
                   await GetAndCachedPlayerLoginsRecordsAsync(playerCode, field);
        }


        private async Task<CachedPlayer> GetCachedPlayerOrNullAsync(string playerCode)
        {
            var values = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Player, playerCode, new RedisValue[]
            {
                CachedPlayerFields.BrandId,
                CachedPlayerFields.ExternalId,
                CachedPlayerFields.Status,
                CachedPlayerFields.Type,
                CachedPlayerFields.Username,
                CachedPlayerFields.DefaultWalletId,
                CachedPlayerFields.CultureCode,
                CachedPlayerFields.BetLimitGroupId,
                CachedPlayerFields.BrandCode,
                CachedPlayerFields.DefaultWalletCurrencyCode,
                CachedPlayerFields.LastTermsUrl,
                CachedPlayerFields.LastCashierUrl,
                CachedPlayerFields.LastHelpUrl,
                CachedPlayerFields.LastLoginUrl,
                CachedPlayerFields.ExternalCurrencyCode
            });

            if (values.All(a => !a.IsNull))
            {
                return new CachedPlayer
                {
                    PlayerCode = playerCode,
                    BrandId = Guid.Parse(values[0]),
                    ExternalId = values[1],
                    Status = (PlayerStatus)(int)values[2],
                    Type = (PlayerTypes)(int)values[3],
                    Username = values[4],
                    DefaultWalletId = Guid.Parse(values[5]),
                    CultureCode = values[6],
                    BetLimitGroupId = (int)values[7],
                    BrandCode = values[8],
                    DefaultWalletCurrencyCode = values[9],
                    LastTermsUrl = values[10],
                    LastCashierUrl = values[11],
                    LastHelpUrl = values[12],
                    LastLoginUrl = values[13],
                    ExternalCurrencyCode = values[14]
                };
            }

            return null;
        }

        private async Task CachePlayerAsync(string playerCode, CachedPlayer player)
        {
            var fields = new HashEntry[]
            {
                new (CachedPlayerFields.BrandId, player.BrandId.ToString()),
                new (CachedPlayerFields.BrandCode, player.BrandCode),
                new (CachedPlayerFields.ExternalId, player.ExternalId),
                new (CachedPlayerFields.Type, (int) player.Type),
                new (CachedPlayerFields.Username, player.Username),
                new (CachedPlayerFields.Status, (int) player.Status),
                new (CachedPlayerFields.DefaultWalletId, player.DefaultWalletId.ToString()),
                new (CachedPlayerFields.DefaultWalletCurrencyCode, player.DefaultWalletCurrencyCode),
                new (CachedPlayerFields.CultureCode, player.CultureCode),
                new (CachedPlayerFields.BetLimitGroupId, player.BetLimitGroupId),
                new (CachedPlayerFields.LastLoginUrl, player.LastLoginUrl ?? string.Empty),
                new (CachedPlayerFields.LastCashierUrl, player.LastCashierUrl ?? string.Empty),
                new (CachedPlayerFields.LastHelpUrl, player.LastHelpUrl ?? string.Empty),
                new (CachedPlayerFields.LastTermsUrl, player.LastTermsUrl ?? string.Empty),
                new (CachedPlayerFields.ExternalCurrencyCode, player.ExternalCurrencyCode)
            };

            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.Player, playerCode, fields, TimeSpan.FromHours(2), true);
        }

        private async Task<CachedPlayerLoginRecord[]> GetAndCachedPlayerLoginsRecordsAsync(string playerCode, string field)
        {
            var results = await _playerQueriesFactory().GetPlayerLoginsRecordsInOneDayAsync(playerCode, LoginStatus.Enabled);
            if (results == null || results.Length == 0)
            {
                return null;
            }

            var loginRecords = results.Select(r => new CachedPlayerLoginRecord
            {
                PlatformType = r.PlatformType,
                ExpiredOn = r.ExpiredOn
            }).ToArray();

            var expiresIn = loginRecords.Max(r => r.ExpiredOn) - DateTimeOffset.UtcNow;
            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.PlayerLoginRecords, playerCode, new HashEntry[] {
                    new (field, _json.SerializeToString(loginRecords))
                }, expiresIn);

            return loginRecords;
        }

        private static class CachedPlayerFields
        {
            public const string BrandId = "brandId";
            public const string BrandCode = "brandCode";
            public const string ExternalId = "externalId";
            public const string Type = "type";
            public const string Username = "username";
            public const string Status = "status";
            public const string DefaultWalletId = "defaultWalletId";
            public const string DefaultWalletCurrencyCode = "defaultWalletCurrencyCode";
            public const string CultureCode = "cultureCode";
            public const string BetLimitGroupId = "betLimitGroupId";
            public const string ExternalCurrencyCode = "externalCurrencyCode";

            public const string LastLoginUrl = "lastLoginUrl";
            public const string LastCashierUrl = "lastCashierUrl";
            public const string LastHelpUrl = "lastHelpUrl";
            public const string LastTermsUrl = "lastTermsUrl";
        }
    }
}