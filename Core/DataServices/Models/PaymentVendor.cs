using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class PaymentVendor : BaseEntity
    {
        public string Description { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
        public virtual ICollection<PaymentChannel> PaymentChannels { get; set; }
    }
}