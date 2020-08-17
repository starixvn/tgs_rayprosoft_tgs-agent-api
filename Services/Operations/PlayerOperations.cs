using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;

namespace TGS.Services.Operations
{
    public interface IPlayerOperations
    {
        Task<CachedPlayer> GetCachedPlayerAsync(string playerCode);
    }

    public sealed class PlayerOperations : IPlayerOperations
    {
        private readonly ITgsRepository _repository;

        public PlayerOperations(ITgsRepository repository)
        {
            _repository = repository;
        }

        async Task<CachedPlayer> IPlayerOperations.GetCachedPlayerAsync(string playerCode)
        {
            var cachedPlayer = await GetCachedPlayerOrNullAsync(playerCode);
            if (cachedPlayer != null)
            {
                return cachedPlayer;
            }

            return null;
        }

        private async Task<CachedPlayer> GetCachedPlayerOrNullAsync(string playerCode)
        {
            //try
            //{
            //    var db = _redisRepository.GetDb();
            //    var values = await db.HashGetAsync(GetCachedPlayerCacheKey(playerId), new RedisValue[]
            //    {
            //        CachedPlayerFields.BrandId,
            //        CachedPlayerFields.ExternalId,
            //        CachedPlayerFields.Status,
            //        CachedPlayerFields.Type,
            //        CachedPlayerFields.Username,
            //        CachedPlayerFields.DefaultWalletId,
            //        CachedPlayerFields.CultureCode,
            //        CachedPlayerFields.BetLimitGroupId,
            //        CachedPlayerFields.BrandCode,
            //        CachedPlayerFields.DefaultWalletCurrencyCode,
            //        CachedPlayerFields.LastTermsUrl,
            //        CachedPlayerFields.LastCashierUrl,
            //        CachedPlayerFields.LastHelpUrl,
            //        CachedPlayerFields.LastLoginUrl,
            //        CachedPlayerFields.BrandProxyWalletConfigurationId,

            //        CachedPlayerFields.PlayerCode,
            //        CachedPlayerFields.ExternalCurrencyCode,
            //    });
            //    if (values.All(a => !a.IsNull))
            //    {
            //        return new CachedPlayer
            //        {
            //            Code = playerCode,
            //            BrandId = Guid.Parse(values[0]),
            //            ExternalId = values[1],
            //            Status = (PlayerStatus)(int)values[2],
            //            Type = (PlayerTypes)(int)values[3],
            //            Username = values[4],
            //            DefaultWalletId = Guid.Parse(values[5]),
            //            CultureCode = values[6],
            //            BetLimitGroupId = (int)values[7],
            //            BrandCode = values[8],
            //            DefaultWalletCurrencyCode = values[9],
            //            LastTermsUrl = values[10],
            //            LastCashierUrl = values[11],
            //            LastHelpUrl = values[12],
            //            LastLoginUrl = values[13],
            //            BrandProxyWalletConfigurationId = values[14].HasValue ? Guid.Parse(values[14]) : (Guid?)null,
            //            PlayerCode = values[15],
            //            ExternalCurrencyCode = values[16]
            //        };
            //    }
            //}
            //catch (Exception e)
            //{
            //    _log.LogError($"Failed to get cached player for playerId: {playerId}", e);
            //}

            return null;
        }
    }
}
