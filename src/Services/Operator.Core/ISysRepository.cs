using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface ISysRepository
    {
        IAsyncEnumerable<KeyValuePair<string, string>> QueryAllLocalizationStringsAsync(string cultureCode);

        IAsyncEnumerable<BannerInfo> QueryBrandBannersAsync(Guid brandId);

        ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId);
    }
}
