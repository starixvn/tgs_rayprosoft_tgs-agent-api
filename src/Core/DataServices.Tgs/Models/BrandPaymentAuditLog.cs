using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandPaymentAuditLog
    {
        public Guid Id { get; set; }

        public Guid BrandPaymentId { get; set; }

        public string Before { get; set; }

        public string After { get; set; }

        public string Reason { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public BrandPaymentOperation Operation { get; set; }
    }
}