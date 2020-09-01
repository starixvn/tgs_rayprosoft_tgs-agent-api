using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandBetLimit
    {
        public int BetLimitGroupId { get; set; }

        public BetLimitGroup BetLimitGroup { get; set; }

        public Guid BrandId { get; set; }

        public Brand Brand { get; set; }
    }
}