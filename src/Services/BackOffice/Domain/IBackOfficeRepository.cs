using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public interface IBackOfficeRepository
    {
        ValueTask<Guid> CreateBannerAsync(BannerCreateInfo createInfo, string Modifier);

        IAsyncEnumerable<LocalizationStringInfo> ExportLocalizationStringsAsync(string systemCode);

        ValueTask ImportLocalizationStringsAsync(string systemCode, IEnumerable<LocalizationStringInfo> localizationStrings);
    }
}
