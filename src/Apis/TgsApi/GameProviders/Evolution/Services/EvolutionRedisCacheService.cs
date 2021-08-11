using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public interface IEvolutionRedisCacheService
    {
        ValueTask AddSidAsync(string sid, Player player);
        ValueTask<string> GetPlayerCodeAsync(string sid);
    }

    public class EvolutionRedisCacheService : IEvolutionRedisCacheService
    {
        private readonly string _sidCacheType = "Evolution:Sid";

        private readonly IRedisCacheManager _redisCacheManager;

        public EvolutionRedisCacheService(IRedisCacheManager redisCacheManager)
        {
            _redisCacheManager = redisCacheManager ?? throw new ArgumentNullException(nameof(redisCacheManager));
        }

        async ValueTask IEvolutionRedisCacheService.AddSidAsync(string sid, Player player)
        {
            await _redisCacheManager.SafeCacheForeverAsync(_sidCacheType, sid, player.PlayerCode).ConfigureAwait(false);
        }

        async ValueTask<string> IEvolutionRedisCacheService.GetPlayerCodeAsync(string sid)
        {
            return await _redisCacheManager.SafeGetAsync(_sidCacheType, sid, () => Task.FromResult(default(string))).ConfigureAwait(false);
        }
    }
}
