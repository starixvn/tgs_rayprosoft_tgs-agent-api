using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class PaymentMethod : BaseEntity<Guid>
    {
        public string Description { get; set; }
        public Guid PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<PaymentChannel> PaymentChannels { get; set; }
    }
}