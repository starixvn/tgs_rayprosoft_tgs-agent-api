using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandMaintenance
{
    public class BatchUpdateBrandMaintenanceRequest
    {
        public TimeOnly BeginTime { get; init; }

        public Guid[] BrandIds { get; init; }

        public TimeOnly EndTime { get; init; }
    }
}
