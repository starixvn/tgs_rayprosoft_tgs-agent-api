using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public interface ISexyGamingRedisCacheService
    {
        ValueTask AddCancelBetCacheAsync(SexyGamingCancelBet[] source);
        ValueTask<string> GetCancelBetPlatformTxIdJoinAsync(SexyGamingBet[] source);
    }
    public class SexyGamingRedisCacheService : ISexyGamingRedisCacheService
    {
        private readonly string _cancelBetCacheType = "SexyGaming:CancelBet";
        private readonly IRedisCacheManager _redisCacheManager;

        public SexyGamingRedisCacheService(IRedisCacheManager redisCacheManager)
        {
            _redisCacheManager = redisCacheManager ?? throw new ArgumentNullException(nameof(redisCacheManager));
        }

        async ValueTask ISexyGamingRedisCacheService.AddCancelBetCacheAsync(SexyGamingCancelBet[] source)
        {
            string platformTxIdJoin = string.Join('|', source.Select(x => x.PlatformTxId).ToArray());

            await _redisCacheManager.SafeCacheAsync(_cancelBetCacheType, platformTxIdJoin, TimeSpan.FromDays(1), platformTxIdJoin).ConfigureAwait(false);
        }

        async ValueTask<string> ISexyGamingRedisCacheService.GetCancelBetPlatformTxIdJoinAsync(SexyGamingBet[] source)
        {
            string platformTxIdJoin = string.Join('|', source.Select(x => x.PlatformTxId).ToArray());
            return await _redisCacheManager.SafeGetAsync(_cancelBetCacheType, platformTxIdJoin, () => Task.FromResult(default(string))).ConfigureAwait(false);
        }
    }
}
