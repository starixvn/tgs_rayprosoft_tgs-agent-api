using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public interface IBackOfficeRepository
    {
        ValueTask ChangeBannerOrderAsync(Guid id, int order, string modifier);

        ValueTask<bool> CheckBrandEmailAsync(Guid brandId, string email);

        ValueTask<Guid> CreateBannerAsync(BannerModifyInfo createInfo, string modifier);

        IAsyncEnumerable<LocalizationStringInfo> ExportLocalizationStringsAsync(string systemCode);

        ValueTask<BannerInfo> GetBannerByIdAsync(Guid id);

        ValueTask ImportLocalizationStringsAsync(string systemCode, IEnumerable<LocalizationStringInfo> localizationStrings);

        IAsyncEnumerable<GameData> QueryAllGamesAsync(Guid brandId);

        IAsyncEnumerable<BannerInfo> QueryExpiredBannersAsync(Guid brandId);

        IAsyncEnumerable<BannerInfo> QueryRejectedBannersAsync(Guid brandId);

        IAsyncEnumerable<BannerInfo> QueryReviewingBannersAsync(Guid brandId);

        IAsyncEnumerable<BannerInfo> QueryValidBannersAsync(Guid brandId);

        ValueTask ReviewBannerAsync(Guid id, BannerReviewState reviewState, string remark, string reviewer);

        ValueTask SetIsEnableBannerAsync(Guid id, bool isEnable, string modifier);

        ValueTask UpdateBannerAsync(Guid id, BannerModifyInfo updateInfo, string modifier);
    }
}
