using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
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
        private readonly Func<IWalletQueries> _walletQueriesFactory;
        private readonly Func<IPlayerQueries> _playerQueriesFactory;
        private readonly IPlayerOperations _interface;

        public PlayerOperations(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            Func<IWalletQueries> walletQueriesFactory,
            Func<IPlayerQueries> playerQueriesFactory)
        {
            _cacheManager = cacheManager;
            _json = json;
            _walletQueriesFactory = walletQueriesFactory;
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
            return _walletQueriesFactory().GetBalanceAsync(walletId, dirtyRead);
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
                CachedPlayerFields.LicenseeId,
                CachedPlayerFields.ExternalId,
                CachedPlayerFields.Status,
                CachedPlayerFields.Type,
                CachedPlayerFields.Username,
                CachedPlayerFields.DefaultWalletId,
                CachedPlayerFields.CultureCode,
                CachedPlayerFields.BetLimitGroupId,
                CachedPlayerFields.LicenseeCode,
                CachedPlayerFields.DefaultWalletCurrencyCode,
                CachedPlayerFields.ExternalCurrencyCode
            });

            if (values.All(a => !a.IsNull))
            {
                return new CachedPlayer
                {
                    PlayerCode = playerCode,
                    LicenseeId = Guid.Parse(values[0]),
                    ExternalId = values[1],
                    Status = (PlayerStatus)(int)values[2],
                    Type = (PlayerTypes)(int)values[3],
                    Username = values[4],
                    DefaultWalletId = Guid.Parse(values[5]),
                    CultureCode = values[6],
                    BetLimitGroupId = (int)values[7],
                    LicenseeCode = values[8],
                    DefaultWalletCurrencyCode = values[9],
                    ExternalCurrencyCode = values[10]
                };
            }

            return null;
        }

        private async Task CachePlayerAsync(string playerCode, CachedPlayer player)
        {
            var fields = new HashEntry[]
            {
                new (CachedPlayerFields.LicenseeId, player.LicenseeId.ToString()),
                new (CachedPlayerFields.LicenseeCode, player.LicenseeCode),
                new (CachedPlayerFields.ExternalId, player.ExternalId),
                new (CachedPlayerFields.Type, (int) player.Type),
                new (CachedPlayerFields.Username, player.Username),
                new (CachedPlayerFields.Status, (int) player.Status),
                new (CachedPlayerFields.DefaultWalletId, player.DefaultWalletId.ToString()),
                new (CachedPlayerFields.DefaultWalletCurrencyCode, player.DefaultWalletCurrencyCode),
                new (CachedPlayerFields.CultureCode, player.CultureCode),
                new (CachedPlayerFields.BetLimitGroupId, player.BetLimitGroupId),
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
            public const string LicenseeId = "licenseeId";
            public const string LicenseeCode = "licenseeCode";
            public const string ExternalId = "externalId";
            public const string Type = "type";
            public const string Username = "username";
            public const string Status = "status";
            public const string DefaultWalletId = "defaultWalletId";
            public const string DefaultWalletCurrencyCode = "defaultWalletCurrencyCode";
            public const string CultureCode = "cultureCode";
            public const string BetLimitGroupId = "betLimitGroupId";
            public const string ExternalCurrencyCode = "externalCurrencyCode";
        }
    }
}