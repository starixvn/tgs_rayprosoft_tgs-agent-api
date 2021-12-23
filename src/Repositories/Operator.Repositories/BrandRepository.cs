using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{
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

            if (brand == null)
                return null;

            var brandCurrency = await _tgsRepository.BrandCurrencies.AsQueryable()
                .Where(bc => bc.BrandId == brand.Id)
                .FirstAsync()
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(brand.LogoInfo))
            {
                return new BrandInfo(
                    brandId,
                    brand.Code,
                    default!,
                    default!,
                    default!,
                    brand.Status,
                    brandCurrency.CurrencyCode,
                    brand.EnableSms,
                    brand.EnableSmsRegisterFlow);
            }

            var logo = JsonSerializer.Deserialize<BrandSmsVendorSettings>(brand.LogoInfo)!;

            return new BrandInfo(
                brand.Id,
                brand.Code,
                logo.DesktopLogoUrl,
                logo.MobileLogoUrl,
                logo.LoadingUrl,
                brand.Status,
                brandCurrency.CurrencyCode,
                brand.EnableSms,
                brand.EnableSmsRegisterFlow);
        }
    }
}
