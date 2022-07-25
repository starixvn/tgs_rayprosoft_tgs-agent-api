using System;

namespace TGS.Services.BackOffice.Application.Services
{
    public record UpdateBrandMaintenanceCommand(Guid Id, string Content);
}
