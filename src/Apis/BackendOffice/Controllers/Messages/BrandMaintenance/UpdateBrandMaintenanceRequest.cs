using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandMaintenance
{
    public class UpdateBrandMaintenanceRequest
    {
        public DateTimeOffset BeginTime { get; init; }

        public DateTimeOffset EndTime { get; init; }

        public string[] BrandCodes { get; init; }
    }
}
