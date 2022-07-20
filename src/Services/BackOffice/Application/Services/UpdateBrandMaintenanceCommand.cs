using System;

namespace TGS.Services.BackOffice.Application.Services
{
    public record BatchUpdateBrandMaintenanceCommand(
        string[] BrandCodes,
        DateTimeOffset BeginTime,
        DateTimeOffset EndTime);
}
