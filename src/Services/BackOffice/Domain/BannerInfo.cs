using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public record BannerInfo(
        Guid Id,
        Guid BrandId,
        string Title,
        DateTimeOffset BeginTime,
        DateTimeOffset? EndTime,
        string Localization,
        int Status,
        bool IsEnabled,
        string Updater,
        DateTimeOffset UpdatedOn);
}
