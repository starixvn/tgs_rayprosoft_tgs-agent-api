using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations.Commands;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IPlayerOperations
    {
        Task<CachedPlayer> GetCachedPlayerAsync(string playerCode);

        Task<decimal> GetPlayerBalanceAsync(Guid defaultWalletId, bool dirtyRead);

        Task<CachedPlayerLoginRecord> GetPlayerLatestEnabledLoginRecordAsync(string playerCode,
            PlatformType? platform = null);

        Task<CachedPlayerLoginRecord[]> GetPlayerEnabledLoginRecordsAsync(string playerCode);

        Task<string> GetPlayerCodeByUsernameAsync(Guid brandId, string username);

        Task<Player> GetPlayerByCodeAsync(string playerCode);

        Task<string> CreatePlayerAsync(CreatePlayerData data);

        Task UpdatePlayerAsync(UpdatePlayerData data);

        Task ChangePlayerPasswordAsync(ChangePlayerPasswordData data);

        Task<PlayerLoginRecord> PlayerLoginAsync(PlayerLoginData data);

        Task PlayerLogoutAsync(PlayerLogoutData data);

        Task<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

        Task<Guid> GetMainWalletIdByPlayerCodeAsync(string playerCode);

        Task<string> GetUsernameByPlayerCodeAsync(string playerCode);
    }

    public sealed class PlayerOperations : IPlayerOperations
    {
        private readonly IRedisCacheManager _cacheManager;

        private readonly IPlayerOperations _interface;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerCommands _playerCommands;
        private readonly Func<IPlayerQueries> _playerQueriesFactory;
        private readonly Func<IWalletQueries> _walletQueriesFactory;

        public PlayerOperations(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            Func<IWalletQueries> walletQueriesFactory,
            Func<IPlayerQueries> playerQueriesFactory,
            IPlayerCommands playerCommands)
        {
            _cacheManager = cacheManager;
            _json = json;
            _walletQueriesFactory = walletQueriesFactory;
            _playerQueriesFactory = playerQueriesFactory;
            _playerCommands = playerCommands;

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

        Task<decimal> IPlayerOperations.GetPlayerBalanceAsync(Guid walletId, bool dirtyRead) =>
            _walletQueriesFactory().GetBalanceAsync(walletId, dirtyRead);

        async Task<CachedPlayerLoginRecord> IPlayerOperations.GetPlayerLatestEnabledLoginRecordAsync(string playerCode,
            PlatformType? platform)
        {
            var loginRecords = await _interface.GetPlayerEnabledLoginRecordsAsync(playerCode);

            return loginRecords?.OrderByDescending(r => r.ExpiredOn)
                .FirstOrDefault(r => platform == null || r.PlatformType == platform);
        }

        async Task<CachedPlayerLoginRecord[]> IPlayerOperations.GetPlayerEnabledLoginRecordsAsync(string playerCode)
        {
            var field = "enabled";
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.PlayerLoginRecords, playerCode, field);

            return value.HasValue
                ? _json.DeserializeFromString<CachedPlayerLoginRecord[]>(value)
                : await GetAndCachedPlayerLoginsRecordsAsync(playerCode, field);
        }

        async Task<string> IPlayerOperations.GetPlayerCodeByUsernameAsync(Guid brandId, string username)
        {
            var cacheType = RedisCacheType.UsernameToPlayerCode;
            var cacheKey = brandId;
            var field = username;

            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, cacheKey, username);
            if (value.HasValue)
            {
                return value;
            }

            var playerCode = await _playerQueriesFactory().GetPlayerCodeByUsernameAsync(brandId, username);
            await _cacheManager.SafeCacheHashSetAsync(cacheType, cacheKey, new HashEntry[]
            {
                new(field, playerCode)
            }, TimeSpan.FromHours(2));

            return playerCode;
        }

        async Task<string> IPlayerOperations.CreatePlayerAsync(CreatePlayerData data)
        {
            var playerCode = await _playerCommands.CreatePlayerAsync(data);

            await _cacheManager.SafeCacheAsync(RedisCacheType.UsernameToPlayerCode, data.Username,
                TimeSpan.FromHours(2), playerCode);

            return playerCode;
        }

        async Task IPlayerOperations.UpdatePlayerAsync(UpdatePlayerData data)
        {
            await _playerCommands.UpdatePlayerAsync(data);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, data.PlayerCode);
        }

        async Task IPlayerOperations.ChangePlayerPasswordAsync(ChangePlayerPasswordData data)
        {
            await _playerCommands.ChangePlayerPasswordAsync(data);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, data.PlayerCode);
        }

        async Task<PlayerLoginRecord> IPlayerOperations.PlayerLoginAsync(PlayerLoginData data)
        {
            var record = await _playerCommands.PlayerLoginAsync(data);
            await _cacheManager.ClearCacheAsync(RedisCacheType.PlayerLoginRecords, data.PlayerCode);

            return record;
        }

        async Task IPlayerOperations.PlayerLogoutAsync(PlayerLogoutData data)
        {
            await _playerCommands.PlayerLogoutAsync(data);
            await _cacheManager.ClearCacheAsync(RedisCacheType.PlayerLoginRecords, data.PlayerCode);
        }

        Task<Player> IPlayerOperations.GetPlayerByCodeAsync(string playerCode) =>
            _playerQueriesFactory().GetPlayerByCodeAsync(playerCode);

        Task<Guid> IPlayerOperations.GetBrandIdByPlayerCodeAsync(string playerCode) =>
            GetCachedPlayerValueAsync(playerCode, CachedPlayerFields.BrandId,
                v => Guid.Parse(v),
                cachedPlayer => cachedPlayer?.BrandId ?? Guid.Empty);

        Task<Guid> IPlayerOperations.GetMainWalletIdByPlayerCodeAsync(string playerCode) =>
            GetCachedPlayerValueAsync(playerCode, CachedPlayerFields.DefaultWalletId,
                v => Guid.Parse(v),
                cachedPlayer => cachedPlayer?.DefaultWalletId ?? Guid.Empty);

        Task<string> IPlayerOperations.GetUsernameByPlayerCodeAsync(string playerCode) =>
            GetCachedPlayerValueAsync(playerCode, CachedPlayerFields.Username,
                value => (string) value,
                cachedPlayer => cachedPlayer?.Username);

        #region Private Methods

        private async Task<CachedPlayer> GetCachedPlayerOrNullAsync(string playerCode)
        {
            var values = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Player, playerCode, new RedisValue[]
            {
                CachedPlayerFields.BrandId,
                CachedPlayerFields.Status,
                CachedPlayerFields.Type,
                CachedPlayerFields.Username,
                CachedPlayerFields.DefaultWalletId,
                CachedPlayerFields.CultureCode,
                CachedPlayerFields.BetLimitGroupId,
                CachedPlayerFields.BrandCode,
                CachedPlayerFields.DefaultWalletCurrencyCode,
                CachedPlayerFields.ExternalCurrencyCode
            });

            if (values.All(a => !a.IsNull))
            {
                return new CachedPlayer
                {
                    PlayerCode = playerCode,
                    BrandId = Guid.Parse(values[0]),
                    Status = (PlayerStatus) (int) values[1],
                    Type = (PlayerType) (int) values[2],
                    Username = values[3],
                    DefaultWalletId = Guid.Parse(values[4]),
                    CultureCode = values[5],
                    BetLimitGroupId = (int) values[6],
                    BrandCode = values[7],
                    DefaultWalletCurrencyCode = values[8],
                    ExternalCurrencyCode = values[9]
                };
            }

            return null;
        }

        private async Task CachePlayerAsync(string playerCode, CachedPlayer player)
        {
            var fields = new HashEntry[]
            {
                new(CachedPlayerFields.BrandId, player.BrandId.ToString()),
                new(CachedPlayerFields.BrandCode, player.BrandCode),
                new(CachedPlayerFields.Type, (int) player.Type),
                new(CachedPlayerFields.Username, player.Username),
                new(CachedPlayerFields.Status, (int) player.Status),
                new(CachedPlayerFields.DefaultWalletId, player.DefaultWalletId.ToString()),
                new(CachedPlayerFields.DefaultWalletCurrencyCode, player.DefaultWalletCurrencyCode),
                new(CachedPlayerFields.CultureCode, player.CultureCode),
                new(CachedPlayerFields.BetLimitGroupId, player.BetLimitGroupId),
                new(CachedPlayerFields.ExternalCurrencyCode, player.ExternalCurrencyCode)
            };

            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.Player, playerCode, fields, TimeSpan.FromHours(2),
                true);
        }

        private async Task<CachedPlayerLoginRecord[]> GetAndCachedPlayerLoginsRecordsAsync(string playerCode,
            string field)
        {
            var results = await _playerQueriesFactory()
                .GetPlayerLoginsRecordsInOneDayAsync(playerCode, LoginStatus.Enabled);
            if (results == null || results.Length == 0)
            {
                return Array.Empty<CachedPlayerLoginRecord>();
            }

            var loginRecords = results.Select(r => new CachedPlayerLoginRecord
            {
                PlatformType = r.PlatformType,
                ExpiredOn = r.ExpiredOn
            }).ToArray();

            var expiresIn = loginRecords.Max(r => r.ExpiredOn) - DateTimeOffset.UtcNow;
            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.PlayerLoginRecords, playerCode, new HashEntry[]
            {
                new(field, _json.SerializeToString(loginRecords))
            }, expiresIn);

            return loginRecords;
        }

        private async Task<T> GetCachedPlayerValueAsync<T>(string playerCode, string field, Func<RedisValue, T> cached,
            Func<CachedPlayer, T> noCache)
        {
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Player, playerCode, field);
            if (value.HasValue)
            {
                return cached(value);
            }

            var cachedPlayer = await _interface.GetCachedPlayerAsync(playerCode);
            return noCache(cachedPlayer);
        }

        private static class CachedPlayerFields
        {
            public const string BrandId = "brandId";
            public const string BrandCode = "brandCode";
            public const string Type = "type";
            public const string Username = "username";
            public const string Status = "status";
            public const string DefaultWalletId = "defaultWalletId";
            public const string DefaultWalletCurrencyCode = "defaultWalletCurrencyCode";
            public const string CultureCode = "cultureCode";
            public const string BetLimitGroupId = "betLimitGroupId";
            public const string ExternalCurrencyCode = "externalCurrencyCode";
        }

        #endregion
    }
}