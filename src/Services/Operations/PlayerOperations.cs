using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.DataServices.Tgs.Models;
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
    public sealed class PlayerOperations : IPlayerOperations
    {
        private readonly IAgentQueries _agentQueries;
        private readonly IRedisCacheManager _cacheManager;

        private readonly IPlayerOperations _interface;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerCommands _playerCommands;
        private readonly Func<IPlayerQueries> _playerQueriesFactory;
        private readonly Func<IWalletQueries> _walletQueriesFactory;

        public PlayerOperations(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            IAgentQueries agentQueries,
            Func<IWalletQueries> walletQueriesFactory,
            Func<IPlayerQueries> playerQueriesFactory,
            IPlayerCommands playerCommands)
        {
            _cacheManager = cacheManager;
            _json = json;
            _walletQueriesFactory = walletQueriesFactory;
            _playerQueriesFactory = playerQueriesFactory;
            _playerCommands = playerCommands;
            _agentQueries = agentQueries;
            _interface = this;
        }

        public async Task ChangePlayerPasswordAsync(ChangePlayerPasswordData data)
        {
            await _playerCommands.ChangePlayerPasswordAsync(data);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, data.PlayerCode);
        }

        public async Task<string> CreatePlayerAsync(CreatePlayerData data)
        {
            var playerCode = await _playerCommands.CreatePlayerAsync(data);

            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.UsernameToPlayerCode, data.BrandId,
                new HashEntry[] { new(data.Username, playerCode) }, TimeSpan.FromHours(12), true);

            return playerCode;
        }

        public Task<Guid> GetBrandIdByPlayerCodeAsync(string playerCode) =>
            GetCachedPlayerValueAsync(playerCode, CachedPlayerFields.BrandId,
                v => Guid.Parse(v),
                cachedPlayer => cachedPlayer?.BrandId ?? Guid.Empty);

        public async Task<CachedPlayer> GetCachedPlayerAsync(string playerCode)
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

        public Task<Guid> GetDefaultWalletIdByPlayerCodeAsync(string playerCode) =>
            GetCachedPlayerValueAsync(playerCode, CachedPlayerFields.DefaultWalletId,
                v => Guid.Parse(v),
                cachedPlayer => cachedPlayer?.DefaultWalletId ?? Guid.Empty);

        public Task<decimal> GetPlayerBalanceAsync(Guid walletId, bool dirtyRead) =>
                    _walletQueriesFactory().GetBalanceAsync(walletId);

        public Task<Player> GetPlayerByCodeAsync(string playerCode) =>
            _playerQueriesFactory().GetPlayerByCodeAsync(playerCode);

        public async Task<string> GetPlayerCodeByUsernameAsync(Guid brandId, string username)
        {
            const RedisCacheType cacheType = RedisCacheType.UsernameToPlayerCode;
            var cacheKey = brandId;
            var field = username;

            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, cacheKey, field);
            if (value.HasValue)
            {
                return value;
            }

            var playerCode = await _playerQueriesFactory().GetPlayerCodeByUsernameAsync(brandId, username);
            await _cacheManager.SafeCacheHashSetAsync(cacheType, cacheKey, new HashEntry[] { new(field, playerCode) },
                TimeSpan.FromHours(12));

            return playerCode;
        }

        public async Task<CachedPlayerLoginRecord[]> GetPlayerEnabledLoginRecordsAsync(string playerCode)
        {
            var field = "enabled";
            var value = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.PlayerLoginRecords, playerCode, field);

            return value.HasValue
                ? _json.DeserializeFromString<CachedPlayerLoginRecord[]>(value)
                : await GetAndCachedPlayerLoginsRecordsAsync(playerCode, field);
        }

        public async Task<CachedPlayerLoginRecord> GetPlayerLatestEnabledLoginRecordAsync(string playerCode,
                                    PlatformType? platform)
        {
            var loginRecords = await _interface.GetPlayerEnabledLoginRecordsAsync(playerCode);

            return loginRecords?.OrderByDescending(r => r.ExpiredOn)
                .FirstOrDefault(r => platform == null || r.PlatformType == platform);
        }

        public Task<CachedPlayerLoginRecord> GetPlayerLoginRecordAsync(Guid loginRecordId) =>
            _cacheManager.SafeGetAndCacheAsync(RedisCacheType.PlayerLoginRecords, loginRecordId,
                TimeSpan.FromHours(2), () => _playerQueriesFactory().GetPlayerLoginRecordAsync(loginRecordId));

        public Task<string> GetUsernameByPlayerCodeAsync(string playerCode) =>
            GetCachedPlayerValueAsync(playerCode, CachedPlayerFields.Username,
                value => (string)value,
                cachedPlayer => cachedPlayer?.Username);

        public async Task<PlayerLoginRecord> PlayerLoginAsync(PlayerLoginData data)
        {
            var record = await _playerCommands.PlayerLoginAsync(data);
            await _cacheManager.ClearCacheAsync(RedisCacheType.PlayerLoginRecords, data.PlayerCode);

            return record;
        }

        public async Task PlayerLogoutAsync(PlayerLogoutData data)
        {
            await _playerCommands.DeauthorizePlayerAsync(data.PlayerCode, data.LoginRecordId);
            await _cacheManager.ClearCacheByBatchAsync(new CacheKey[]
            {
                new() {CacheType = RedisCacheType.PlayerLoginRecords, Key = data.PlayerCode},
                new() {CacheType = RedisCacheType.PlayerLoginRecords, Key = data.LoginRecordId}
            });
        }

        #region Private Methods

        public async Task SetEmailAsync(string playerCode, string email, DateTimeOffset updatedOn)
        {
            await _playerCommands.SetEmailAsync(playerCode, email, updatedOn);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, playerCode);
        }

        public async Task SetIdentityAsync(
            string playerCode,
            string realName,
            string identity,
            bool isAuthenticate,
            DateTimeOffset updatedOn)
        {
            await _playerCommands.SetIdentityAsync(playerCode, realName, identity, isAuthenticate, updatedOn);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, playerCode);
        }

        public async Task SetPayPasswordAsync(string playerCode, byte[] newPassword, byte[] newSalt,
                            DateTimeOffset updatedOn)
        {
            await _playerCommands.SetPayPasswordAsync(playerCode, newPassword, newSalt, updatedOn);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, playerCode);
        }

        public async Task SetPhoneAsync(string playerCode, string phone, DateTimeOffset updatedOn)
        {
            await _playerCommands.SetPhoneAsync(playerCode, phone, updatedOn);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, playerCode);
        }

        public async Task SetQqAsync(string playerCode, string qqId, DateTimeOffset updatedOn)
        {
            await _playerCommands.SetQqAsync(playerCode, qqId, updatedOn);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, playerCode);
        }

        public async Task SetWeChatAsync(string playerCode, string weChatId, DateTimeOffset updatedOn)
        {
            await _playerCommands.SetWeChatAsync(playerCode, weChatId, updatedOn);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Player, playerCode);
        }

        private async Task CachePlayerAsync(string playerCode, CachedPlayer player)
        {
            var fields = new HashEntry[]
            {
                new(CachedPlayerFields.BrandId, player.BrandId.ToString()),
                new(CachedPlayerFields.BrandCode, player.BrandCode), new(CachedPlayerFields.Type, (int)player.Type),
                new(CachedPlayerFields.Username, player.Username),
                new(CachedPlayerFields.Status, (int)player.Status),
                new(CachedPlayerFields.DefaultWalletId, player.DefaultWalletId.ToString()),
                new(CachedPlayerFields.DefaultWalletCurrencyCode, player.DefaultWalletCurrencyCode),
                new(CachedPlayerFields.BetLimitId, player.BetLimitId),
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
                PlayerCode = playerCode,
                PlatformType = r.PlatformType,
                Status = r.Status,
                ExpiredOn = r.ExpiredOn
            }).ToArray();

            var expiresIn = loginRecords.Max(r => r.ExpiredOn) - DateTimeOffset.UtcNow;
            await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.PlayerLoginRecords, playerCode,
                new HashEntry[] { new(field, _json.SerializeToString(loginRecords)) }, expiresIn);

            return loginRecords;
        }

        private async Task<CachedPlayer> GetCachedPlayerOrNullAsync(string playerCode)
        {
            var values = await _cacheManager.SafeGetHashSetAsync(RedisCacheType.Player, playerCode,
                new RedisValue[]
                {
                    CachedPlayerFields.BrandId, CachedPlayerFields.Status, CachedPlayerFields.Type,
                    CachedPlayerFields.Username, CachedPlayerFields.DefaultWalletId, CachedPlayerFields.BetLimitId,
                    CachedPlayerFields.BrandCode, CachedPlayerFields.DefaultWalletCurrencyCode,
                    CachedPlayerFields.ExternalCurrencyCode
                });

            if (values.All(a => !a.IsNull))
            {
                return new()
                {
                    PlayerCode = playerCode,
                    BrandId = Guid.Parse(values[0]),
                    Status = (PlayerStatus)(int)values[1],
                    Type = (PlayerType)(int)values[2],
                    Username = values[3],
                    DefaultWalletId = Guid.Parse(values[4]),
                    BetLimitId = (int)values[5],
                    BrandCode = values[6],
                    DefaultWalletCurrencyCode = values[7],
                    ExternalCurrencyCode = values[8]
                };
            }

            return null;
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
            public const string BetLimitId = "betLimitId";
            public const string BrandCode = "brandCode";
            public const string BrandId = "brandId";
            public const string DefaultWalletCurrencyCode = "defaultWalletCurrencyCode";
            public const string DefaultWalletId = "defaultWalletId";
            public const string ExternalCurrencyCode = "externalCurrencyCode";
            public const string Status = "status";
            public const string Type = "type";
            public const string Username = "username";
        }

        #endregion Private Methods
    }
}
