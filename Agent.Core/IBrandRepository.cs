using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Agent;

public interface IBrandRepository
{
    ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId);

    IAsyncEnumerable<VipLevelInfo> GetVipLevelsAsync(Guid brandId);

    ValueTask<BrandBetLimit> GetBrandDefaultBetLimitAsync(Guid brandId, CancellationToken cancellationToken);
}
