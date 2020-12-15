using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class BrandGroupBetLimit
    {
        public Guid Id { get; set; }

        public int BetLimitId { get; set; }

        public virtual BetLimit BetLimit { get; set; }

        public Guid BrandGroupId { get; set; }

        public virtual BrandGroup BrandGroup { get; set; }

        public virtual ICollection<BrandGroupGameProviderBetLimit> GameProviderBetLimits { get; set; }
    }
}