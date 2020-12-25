using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PaymentVipLevel
    {
        public Guid PaymentId { get; set; }
        public virtual BrandPayment Payment { get; set; }
        public Guid VipLevelId { get; set; }
        public virtual VipLevel VipLevel { get; set; }
    }
}