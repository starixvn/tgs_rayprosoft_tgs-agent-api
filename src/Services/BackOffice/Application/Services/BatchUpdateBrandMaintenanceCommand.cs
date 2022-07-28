using System;

namespace TGS.Services.BackOffice.Application.Services
{
    public record BatchUpdateBrandMaintenanceCommand(
        Guid[] BrandIds,
        TimeOnly BeginTime,
        TimeOnly EndTime);
}
