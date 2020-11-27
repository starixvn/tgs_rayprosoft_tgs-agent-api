using System;

namespace TGS.Core.DataServices.Models
{
    public class ResellerBetLimit
    {
        public int BetLimitGroupId { get; set; }

        public virtual BetLimitGroup BetLimitGroup { get; set; }

        public Guid ResellerId { get; set; }

        public virtual Reseller Reseller { get; set; }
    }
}