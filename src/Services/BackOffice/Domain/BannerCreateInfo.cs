using System;

namespace TGS.Services.BackOffice.Domain
{
    public record BannerCreateInfo(
        Guid BrandId,
        string Title,
        DateTimeOffset BeginTime,
        DateTimeOffset? EndTime,
        string Localization);
}
