using System;

namespace TGS.Core.DataServices.Models
{
    public class PaymentChannel : BaseEntity<Guid>
    {
        public string Description { get; set; }
        public Guid PaymentVendorId { get; set; }
        public virtual Payment PaymentVendor { get; set; }
        public Guid PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}