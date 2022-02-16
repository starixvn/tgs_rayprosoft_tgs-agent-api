using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player
{
    internal class GetPlayerVipLevelsQueryService : IQueryService<GetPlayerVipLevelsQuery, VipLevelInfo>
    {
        private readonly IBrandRepository _brandRepository;

        public GetPlayerVipLevelsQueryService(
            IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public IAsyncEnumerable<VipLevelInfo> QueryAsync(
            GetPlayerVipLevelsQuery query,
            CancellationToken cancellationToken = default)
            => _brandRepository.GetVipLevelsAsync(query.BrandId);
    }
}
