using System;

namespace TGS.Core.DataServices.Models
{
    public class ResellerBetLimit
    {
        public int BetLimitId { get; set; }

        public virtual BetLimit BetLimit { get; set; }

        public Guid ResellerId { get; set; }

        public virtual Reseller Reseller { get; set; }
    }
}