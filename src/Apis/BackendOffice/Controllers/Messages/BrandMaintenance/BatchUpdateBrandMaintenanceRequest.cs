using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandMaintenance
{
    public class BatchUpdateBrandMaintenanceRequest
    {
        public DateTimeOffset BeginTime { get; init; }

        public Guid[] BrandIds { get; init; }

        public DateTimeOffset EndTime { get; init; }
    }
}
