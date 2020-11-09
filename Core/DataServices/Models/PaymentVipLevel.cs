using System;

namespace TGS.Core.DataServices.Models
{
    public class PaymentVipLevel
    {
        public Guid PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public Guid VipLevelId { get; set; }
        public virtual VipLevel VipLevel { get; set; }
    }
}