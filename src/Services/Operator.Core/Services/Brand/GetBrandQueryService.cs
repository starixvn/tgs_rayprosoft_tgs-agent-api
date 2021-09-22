using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Brand
{
    internal class GetBrandQueryService : IGetService<BrandQuery, BrandInfo>
    {
        private readonly ISysRepository _sysRepository;

        public GetBrandQueryService(ISysRepository sysRepository)
        {
            _sysRepository = sysRepository ?? throw new ArgumentNullException(nameof(sysRepository));
        }

        public async ValueTask<BrandInfo> GetAsync(
            BrandQuery query,
            CancellationToken cancellationToken = default)
        {
            var brandInfo = await _sysRepository
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
