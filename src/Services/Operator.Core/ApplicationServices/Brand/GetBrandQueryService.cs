using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.Operator.ApplicationServices.Brand;

namespace TGS.Services.Operator.ApplicationServices.Brand
{
    internal class GetBrandQueryService : IGetService<BrandQuery, BrandInfo>
    {
        private readonly IBrandRepository _brandRepository;

        public GetBrandQueryService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public async ValueTask<BrandInfo> GetAsync(
            BrandQuery query,
            CancellationToken cancellationToken = default)
        {
            var brandInfo = await _brandRepository
                .GetBrandInfoAsync(query.BrandId)
                .ConfigureAwait(false);

            if (brandInfo == null)
            {
                throw new BrandNotFoundException();
            }

            return brandInfo;
        }
    }
}
