using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories
{
    internal class BackOfficeRepository : IBackOfficeRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public BackOfficeRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<Guid> CreateBannerAsync(BannerModifyInfo createInfo, string modifier)
        {
            var createBanner = new Banner
            {
                Id = Guid.NewGuid(),
                BrandId = createInfo.BrandId,
                BeginTime = createInfo.BeginTime,
                CreatedByUserName = modifier,
                UpdatedByUserName = modifier,
                IsEnabled = true,
                EndTime = createInfo.EndTime,
                Localization = createInfo.Localization,
                Title = createInfo.Title
            };

            await _tgsRepository.Banners
                .AddAsync(createBanner)
                .ConfigureAwait(false);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);

            return createBanner.Id;
        }

        public IAsyncEnumerable<LocalizationStringInfo> ExportLocalizationStringsAsync(string systemCode)
                    => _tgsRepository.SystemLocalizations.AsQueryable()
                .Where(loc => loc.SystemCode == systemCode)
                .Select(loc => new LocalizationStringInfo(
                    loc.SystemCode,
                    loc.CultureCode,
                    loc.MessageId,
                    loc.Text))
                .ToAsyncEnumerable();

        public async ValueTask ImportLocalizationStringsAsync(string systemCode, IEnumerable<LocalizationStringInfo> localizationStrings)
        {
            var allLocalizationStrings = _tgsRepository.SystemLocalizations.AsQueryable()
                .Where(loc => loc.SystemCode == systemCode)
                .ToArray();

            var grouping = (from loc in allLocalizationStrings
                            group loc by loc.CultureCode into g
                            select new
                            {
                                CaltureCode = g.Key,
                                Items = g.ToDictionary(loc => loc.MessageId)
                            }).ToDictionary(g => g.CaltureCode, g => g.Items);

            foreach (var info in localizationStrings)
            {
                if (grouping.TryGetValue(info.CultureCode, out var cultureDict)
                    && cultureDict.TryGetValue(info.MessageId, out var localization))
                {
                    localization.Text = info.Text;
                }
                else
                {
                    await _tgsRepository.SystemLocalizations
                        .AddAsync(new SystemLocalization
                        {
                            SystemCode = info.SystemCode,
                            CultureCode = info.CultureCode,
                            MessageId = info.MessageId,
                            Text = info.Text
                        }).ConfigureAwait(false);
                }
            }

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask ReviewBannerAsync(Guid id, BannerReviewState reviewState, string remark, string reviewer)
        {
            var banner = await _tgsRepository.Banners
                .FindAsync(id)
                .ConfigureAwait(false);

            banner.Status = reviewState switch
            {
                BannerReviewState.Approve => 1000,
                BannerReviewState.Reject => 2000,
                _ => 9000
            };

            banner.UpdatedByUserName = reviewer;
            banner.UpdatedOn = DateTimeOffset.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask SetIsEnableBannerAsync(Guid id, bool isEnable, string modifier)
        {
            var banner = await _tgsRepository.Banners
                .FindAsync(id)
                .ConfigureAwait(false);

            banner.IsEnabled = isEnable;

            banner.UpdatedByUserName = modifier;
            banner.UpdatedOn = DateTimeOffset.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateBannerAsync(Guid id, BannerModifyInfo updateInfo, string modifier)
        {
            var banner = await _tgsRepository.Banners
                .FindAsync(id)
                .ConfigureAwait(false);

            banner.Title = updateInfo.Title;
            banner.BeginTime = updateInfo.BeginTime;
            banner.EndTime = updateInfo.EndTime;
            banner.Localization = updateInfo.Localization;

            banner.UpdatedByUserName = modifier;
            banner.UpdatedOn = DateTimeOffset.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
