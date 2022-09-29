using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices;

internal class GetBrandInfoQueryService : IGetService<GetBrandInfoQuery, BrandInfo?>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandInfoQueryService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public ValueTask<BrandInfo?> GetAsync(GetBrandInfoQuery query, CancellationToken cancellationToken = default)
        => _brandRepository.GetBrandInfoAsync(query.BrandId);
}
