using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    public class MemoryGetPlayerQueryService : IGetService<GetPlayerQuery, PlayerInfo>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IGetService<GetPlayerQuery, PlayerInfo> _targetService;

        public MemoryGetPlayerQueryService(
            IGetService<GetPlayerQuery, PlayerInfo> targetService,
            IMemoryCache memoryCache)
        {
            _targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async ValueTask<PlayerInfo> GetAsync(GetPlayerQuery query, CancellationToken cancellationToken = default)
        {
            if (!_memoryCache.TryGetValue(query.PlayerCode, out PlayerInfo player))
            {
                player = await _targetService.GetAsync(query).ConfigureAwait(false);
                _ = _memoryCache.Set(query.PlayerCode, player, TimeSpan.FromHours(1));
            }
            return player;
        }
    }
}
