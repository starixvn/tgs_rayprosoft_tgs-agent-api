using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Brand
{
    internal class GetBrandQueryService : IGetService<BrandQuery, BrandInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetBrandQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<BrandInfo> GetAsync(
            BrandQuery query,
            CancellationToken cancellationToken = default)
        {
            var result = await _tgsRepository.Brands.FindAsync(query.BrandId);

            if (result == null)
            {
                throw new BrandNotFoundException();
            }

            if (result.LogoInfo.IsNullOrEmpty())
            {
                return new BrandInfo(query.BrandId, default, default, default);
            }

            var logo = JsonSerializer.Deserialize<BrandInfo>(result.LogoInfo);

            return new BrandInfo(logo.BrandId, logo.DesktopLogoUrl, logo.MobileLogoUrl, logo.LoadingUrl);
        }
    }
}