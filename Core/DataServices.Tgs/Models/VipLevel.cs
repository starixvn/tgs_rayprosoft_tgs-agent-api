using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class VipLevel : BaseEntity<Guid>
    {
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<PaymentVipLevel> PaymentVipLevels { get; set; }
    }
}