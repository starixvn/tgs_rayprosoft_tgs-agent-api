using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class VipLevel : BaseEntity<Guid>
    {
        public bool IsDefault { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<BrandPaymentVipLevel> PaymentVipLevels { get; set; }
    }
}