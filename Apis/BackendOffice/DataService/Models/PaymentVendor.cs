using System;
using System.Collections.Generic;

namespace TGS.Apis.BackendOffice.DataService.Models
{
    public class PaymentVendor
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
}