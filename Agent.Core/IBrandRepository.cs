using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.Agent;

public interface IBrandRepository
{
    ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId);

    IAsyncEnumerable<VipLevelInfo> GetVipLevelsAsync(Guid brandId);

    ValueTask<int> GetBrandDefaultBetLimitIdAsync(Guid brandId, CancellationToken cancellationToken);
}