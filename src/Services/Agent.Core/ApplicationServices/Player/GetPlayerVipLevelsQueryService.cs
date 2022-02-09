using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.Agent.ApplicationServices.Player
{
    internal class GetPlayerVipLevelsQueryService : IQueryService<GetPlayerVipLevelsQuery, VipLevelInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetPlayerVipLevelsQueryService(
            ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public IAsyncEnumerable<VipLevelInfo> QueryAsync(
            GetPlayerVipLevelsQuery query,
            CancellationToken cancellationToken = default)
            => _tgsRepository.VipLevels
                .AsQueryable()
                .Where(vip => vip.BrandId == query.BrandId)
                .Select(vip => new VipLevelInfo(vip.Id, vip.Level))
                .ToAsyncEnumerable();
    }
}
