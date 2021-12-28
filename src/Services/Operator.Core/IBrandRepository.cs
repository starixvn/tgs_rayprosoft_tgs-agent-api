using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IBrandRepository
    {
        ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId);

        IAsyncEnumerable<AnnouncementInfo> QueryAnnouncementInfosAsync(Guid brandId, DateTimeOffset searchDate);

        IAsyncEnumerable<BannerInfo> QueryBrandBannersAsync(Guid brandId);
    }
}
