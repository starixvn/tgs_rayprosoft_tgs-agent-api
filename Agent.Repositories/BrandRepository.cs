using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Agent;

namespace TGS.Agent.Repositories;

internal class BrandRepository : IBrandRepository
{
    private readonly ITgsRepository _tgsRepository;

    public BrandRepository(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public async ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId)
    {
        var brand = await _tgsRepository.Brands
            .FindAsync(brandId)
            .ConfigureAwait(false);

        return brand is null
            ? null
            : new BrandInfo(
                brand.Id,
                brand.Code,
                brand.EnableAgentSms,
                brand.EnableSmsRegisterFlow,
                brand.AgentDividendType,
                _tgsRepository.BrandDomains
                    .AsQueryable()
                    .FirstOrDefault(bd => bd.BrandId == brand.Id)
                    ?.Url,
                brand.PlatformCharge);
    }

    public IAsyncEnumerable<VipLevelInfo> GetVipLevelsAsync(Guid brandId)
        => _tgsRepository.VipLevels
            .AsQueryable()
            .Where(vip => vip.BrandId == brandId)
            .Select(vip => new VipLevelInfo(vip.Id, vip.Level))
            .ToAsyncEnumerable();

    public async ValueTask<int> GetBrandDefaultBetLimitIdAsync(Guid brandId,
        CancellationToken cancellationToken)
        => (await _tgsRepository.BrandBetLimits.FirstAsync(x => x.BrandId == brandId && x.IsDefault, cancellationToken))
            .BetLimitId;
}
