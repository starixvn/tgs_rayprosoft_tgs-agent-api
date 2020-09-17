using System;

namespace TGS.Core.DataServices.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; }
        public string PlayerCode { get; set; }
        public string Operation { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
