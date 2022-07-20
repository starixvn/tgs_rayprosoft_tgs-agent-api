using System;

namespace TGS.Services.BackOffice.Domain
{
    public record BrandMaintenanceInfo(
        string BrandName,
        string BrandCode,
        string LicenseeName,
        DateTimeOffset? BeginTime = null,
        DateTimeOffset? EndTime = null);
}
