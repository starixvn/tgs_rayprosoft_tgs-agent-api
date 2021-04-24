using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PromotionVipLevel
    {
        public Guid PromotionId { get; set; }
        public Guid VipLevelId { get; set; }
        public int Level { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual VipLevel VipLevel { get; set; }
    }
}