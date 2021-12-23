using System;
using System.Collections.Generic;

namespace TGS.Services.Operator
{
    public interface ISysRepository
    {
        IAsyncEnumerable<KeyValuePair<string, string>> QueryAllLocalizationStringsAsync(string cultureCode);

        IAsyncEnumerable<AnnouncementInfo> QueryAnnouncementInfosAsync(Guid brandId, DateTimeOffset searchDate);

        IAsyncEnumerable<BannerInfo> QueryBrandBannersAsync(Guid brandId);
    }
}
