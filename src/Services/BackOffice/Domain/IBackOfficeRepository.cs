using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public interface IBackOfficeRepository
    {
        ValueTask<Guid> CreateBannerAsync(BannerModifyInfo createInfo, string modifier);

        IAsyncEnumerable<LocalizationStringInfo> ExportLocalizationStringsAsync(string systemCode);

        ValueTask ImportLocalizationStringsAsync(string systemCode, IEnumerable<LocalizationStringInfo> localizationStrings);

        ValueTask ReviewBannerAsync(Guid id, BannerReviewState reviewState, string remark, string reviewer);

        ValueTask SetIsEnableBannerAsync(Guid id, bool isEnable, string modifier);

        ValueTask UpdateBannerAsync(Guid id, BannerModifyInfo updateInfo, string modifier);
    }
}
