using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandBetLimit
    {
        public int BetLimitGroupId { get; set; }

        public virtual BetLimitGroup BetLimitGroup { get; set; }

        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }
    }
}