using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class PaymentMethod : BaseEntity<Guid>
    {
        public string Description { get; set; }
        public Guid PaymentVendorId { get; set; }
        public virtual Payment PaymentVendor { get; set; }
        public virtual ICollection<PaymentChannel> PaymentChannels { get; set; }
    }
}