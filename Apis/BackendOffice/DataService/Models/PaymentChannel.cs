using System;

namespace TGS.Apis.BackendOffice.DataService.Models
{
    public class PaymentChannel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid PaymentVendorId { get; set; }
        public PaymentVendor PaymentVendor { get; set; }
        public Guid PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}