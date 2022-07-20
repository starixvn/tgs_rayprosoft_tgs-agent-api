using System;

namespace TGS.Services.BackOffice.Domain
{
    public record BrandMaintenanceInfo(
        string BrandName,
        string BrandCode,
        string LicenseeName,
        string Description,
        Guid Id,
        string Content,
        DateTimeOffset BeginTime,
        DateTimeOffset EndTime);
}
