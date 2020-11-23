using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandGameProvider
{
    public class SetScheduledMaintenanceTimeRequest
    {
        public MaintenanceTime[] MaintenanceTimes { get; set; }
    }

    public class MaintenanceTime
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}