using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class PaymentMethod : BaseEntity
    {
        public string Description { get; set; }
        public Guid PaymentVendorId { get; set; }
        public virtual PaymentVendor PaymentVendor { get; set; }
        public virtual ICollection<PaymentChannel> PaymentChannels { get; set; }
    }
}