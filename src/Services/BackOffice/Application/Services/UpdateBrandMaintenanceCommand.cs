using System;

namespace TGS.Services.BackOffice.Application.Services
{
    public record BatchUpdateBrandMaintenanceCommand(
        Guid[] Ids,
        DateTimeOffset BeginTime,
        DateTimeOffset EndTime);
}
