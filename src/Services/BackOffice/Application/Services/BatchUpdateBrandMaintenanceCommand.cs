using System;

namespace TGS.Services.BackOffice.Application.Services
{
    public record BatchUpdateBrandMaintenanceCommand(
        Guid[] BrandIds,
        DateTimeOffset BeginTime,
        DateTimeOffset EndTime);
}
