using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class VipLevel : BaseEntity
    {
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<PaymentVipLevel> PaymentVipLevels { get; set; }
    }
}