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
        private readonly ISystemClock _systemClock;
        private readonly ITgsRepository _tgsRepository;

        public BackOfficeRepository(ITgsRepository tgsRepository, ISystemClock systemClock)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
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

        public IAsyncEnumerable<BannerInfo> QueryExpiredBannersAsync(Guid brandId)
        {
            var currentTime = _systemClock.UtcNow;

            var query = from banner in _tgsRepository.Banners.AsQueryable()
                        where banner.BrandId == brandId
                            && banner.Status == 1000
                            && banner.EndTime <= currentTime
                        select new BannerInfo(
                            banner.Id,
                            banner.BrandId,
                            banner.Title,
                            banner.BeginTime,
                            banner.EndTime,
                            banner.Localization,
                            ConvertBannerStatus(banner.Status),
                            banner.IsEnabled,
                            banner.DisplayOrder,
                            banner.UpdatedByUserName,
                            banner.UpdatedOn);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<BannerInfo> QueryRejectedBannersAsync(Guid brandId)
        {
            var query = from banner in _tgsRepository.Banners.AsQueryable()
                        where banner.BrandId == brandId
                            && banner.Status == 2000
                        select new BannerInfo(
                            banner.Id,
                            banner.BrandId,
                            banner.Title,
                            banner.BeginTime,
                            banner.EndTime,
                            banner.Localization,
                            ConvertBannerStatus(banner.Status),
                            banner.IsEnabled,
                            banner.DisplayOrder,
                            banner.UpdatedByUserName,
                            banner.UpdatedOn);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<BannerInfo> QueryReviewingBannersAsync(Guid brandId)
        {
            var query = from banner in _tgsRepository.Banners.AsQueryable()
                        where banner.BrandId == brandId
                            && banner.Status == 0
                        select new BannerInfo(
                            banner.Id,
                            banner.BrandId,
                            banner.Title,
                            banner.BeginTime,
                            banner.EndTime,
                            banner.Localization,
                            ConvertBannerStatus(banner.Status),
                            banner.IsEnabled,
                            banner.DisplayOrder,
                            banner.UpdatedByUserName,
                            banner.UpdatedOn);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<BannerInfo> QueryValidBannersAsync(Guid brandId)
        {
            var currentTime = _systemClock.UtcNow;

            var query = from banner in _tgsRepository.Banners.AsQueryable()
                        where banner.BrandId == brandId
                            && banner.Status == 1000
                            && (banner.EndTime == null || banner.EndTime > currentTime)
                        select new BannerInfo(
                            banner.Id,
                            banner.BrandId,
                            banner.Title,
                            banner.BeginTime,
                            banner.EndTime,
                            banner.Localization,
                            ConvertBannerStatus(banner.Status),
                            banner.IsEnabled,
                            banner.DisplayOrder,
                            banner.UpdatedByUserName,
                            banner.UpdatedOn);

            return query.ToAsyncEnumerable();
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
            banner.UpdatedOn = _systemClock.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask SetIsEnableBannerAsync(Guid id, bool isEnable, string modifier)
        {
            var banner = await _tgsRepository.Banners
                .FindAsync(id)
                .ConfigureAwait(false);

            banner.IsEnabled = isEnable;

            banner.UpdatedByUserName = modifier;
            banner.UpdatedOn = _systemClock.UtcNow;

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
            banner.UpdatedOn = _systemClock.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        private BannerFlowStatus ConvertBannerStatus(int status)
            => status switch
            {
                0 => BannerFlowStatus.WaitApprving,
                1000 => BannerFlowStatus.Approved,
                2000 => BannerFlowStatus.Rejected,
                _ => BannerFlowStatus.Unknown
            };
    }
}
