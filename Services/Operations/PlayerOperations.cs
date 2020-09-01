using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IPlayerOperations
    {
        Task<CachedPlayer> GetCachedPlayerAsync(string playerCode);
        Task<decimal> GetPlayerBalanceAsync(Guid defaultWalletId, bool dirtyRead);
    }

    public sealed class PlayerOperations : IPlayerOperations
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IWalletQueries _walletQueries;
        private readonly Func<IPlayerQueries> _playerQueriesFactory;

        public PlayerOperations( 
            IRedisCacheManager cacheManager,
            IWalletQueries walletQueries,
            Func<IPlayerQueries> playerQueriesFactory)
        {
            _cacheManager = cacheManager;
            _walletQueries = walletQueries;
            _playerQueriesFactory = playerQueriesFactory;
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
                //throw new PlayerNotFoundException($"Player not found with code: {playerCode}");
            }

            await CachePlayerAsync(playerCode, cachedPlayer);
            
            return cachedPlayer;
        }

        Task<decimal> IPlayerOperations.GetPlayerBalanceAsync(Guid walletId, bool dirtyRead)
        {
            return _walletQueries.GetBalanceAsync(walletId, dirtyRead);
        }

        private async Task<CachedPlayer> GetCachedPlayerOrNullAsync(string playerCode)
        {
            try
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
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to get cached player for player code: {playerCode}", e);
            }

            return null;
        }
        
        private async Task CachePlayerAsync(string playerCode, CachedPlayer player)
        {
            try
            {
                var fields = new[]
                {
                    new HashEntry(CachedPlayerFields.BrandId, player.BrandId.ToString()),
                    new HashEntry(CachedPlayerFields.BrandCode, player.BrandCode),
                    new HashEntry(CachedPlayerFields.ExternalId, player.ExternalId),
                    new HashEntry(CachedPlayerFields.Type, (int) player.Type),
                    new HashEntry(CachedPlayerFields.Username, player.Username),
                    new HashEntry(CachedPlayerFields.Status, (int) player.Status),
                    new HashEntry(CachedPlayerFields.DefaultWalletId, player.DefaultWalletId.ToString()),
                    new HashEntry(CachedPlayerFields.DefaultWalletCurrencyCode, player.DefaultWalletCurrencyCode),
                    new HashEntry(CachedPlayerFields.CultureCode, player.CultureCode),
                    new HashEntry(CachedPlayerFields.BetLimitGroupId, player.BetLimitGroupId),
                    new HashEntry(CachedPlayerFields.LastLoginUrl, player.LastLoginUrl ?? string.Empty),
                    new HashEntry(CachedPlayerFields.LastCashierUrl, player.LastCashierUrl ?? string.Empty),
                    new HashEntry(CachedPlayerFields.LastHelpUrl, player.LastHelpUrl ?? string.Empty),
                    new HashEntry(CachedPlayerFields.LastTermsUrl, player.LastTermsUrl ?? string.Empty),
                    new HashEntry(CachedPlayerFields.ExternalCurrencyCode, player.ExternalCurrencyCode)
                };

                await _cacheManager.SafeCacheHashSetAsync(RedisCacheType.Player, playerCode, fields, TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to cache player for playerId: {playerCode}", e);
            }
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
