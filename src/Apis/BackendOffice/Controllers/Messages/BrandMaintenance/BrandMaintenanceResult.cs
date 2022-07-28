using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandMaintenance
{
    public class BrandMaintenanceResult
    {
        public TimeOnly? BeginTime { get; set; }

        public string BrandCode { get; set; }

        public string BrandName { get; set; }

        public TimeOnly? EndTime { get; set; }

        public Guid? Id { get; set; }

        public string LicenseeName { get; set; }
        public Guid BrandId { get; set; }
    }
}
