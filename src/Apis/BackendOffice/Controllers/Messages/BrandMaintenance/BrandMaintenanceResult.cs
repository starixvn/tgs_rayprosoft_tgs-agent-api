using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandMaintenance
{
    public class BrandMaintenanceResult
    {
        public Guid Id { get; set; }
        public string LicenseeName { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public DateTimeOffset BeginTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
