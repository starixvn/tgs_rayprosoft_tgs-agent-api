using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{
    internal class SysRepository : ISysRepository
    {
        private readonly ISystemClock _systemClock;
        private readonly ITgsRepository _tgsRepository;

        public SysRepository(ITgsRepository tgsRepository, ISystemClock systemClock)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public IAsyncEnumerable<KeyValuePair<string, string>> QueryAllLocalizationStringsAsync(string cultureCode)
        {
            var rows = _tgsRepository.SystemLocalizations.AsQueryable()
                .Where(loc => loc.SystemCode == "OP-API" && loc.CultureCode == cultureCode)
                .Select(loc => new
                {
                    MessageId = loc.MessageId,
                    Text = loc.Text
                });

            return rows.ToAsyncEnumerable()
                .Select(item => new KeyValuePair<string, string>(item.MessageId, item.Text));
        }

        public IAsyncEnumerable<BannerInfo> QueryBrandBannersAsync(Guid brandId)
        {
            var currentTime = _systemClock.UtcNow;

            var rows = from banner in _tgsRepository.Banners.AsQueryable()
                       where banner.Status == 1000
                        && banner.BrandId == brandId
                        && banner.BeginTime <= currentTime
                        && (banner.EndTime == null || banner.EndTime > currentTime)
                        && banner.IsEnabled
                       orderby banner.DisplayOrder
                       select new BannerInfo(
                           banner.Id,
                           banner.Title,
                           banner.Localization);

            return rows.ToAsyncEnumerable();
        }

        public async ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId)
        {
            var brand = await _tgsRepository.Brands
                .FindAsync(brandId)
                .ConfigureAwait(false);

            if (brand == null)
                return null;

            if (string.IsNullOrWhiteSpace(brand.LogoInfo))
            {
                return new BrandInfo(
                    brandId,
                    default!,
                    default!,
                    default!,
                    brand.EnableSms);
            }

            var logo = JsonSerializer.Deserialize<BrandSmsVendorSettings>(brand.LogoInfo)!;

            return new BrandInfo(
                brand.Id,
                logo.DesktopLogoUrl,
                logo.MobileLogoUrl,
                logo.LoadingUrl,
                brand.EnableSms);
        }
    }
}
