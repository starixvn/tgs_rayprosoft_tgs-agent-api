using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class SportMarketType
    {
        public Guid Id { get; set; }

        public Guid SportTypeId { get; set; }

        public virtual SportType SportType { get; set;}

        public Guid MarketTypeId { get; set; }

        public virtual MarketType MarketType { get; set;}

        public virtual ICollection<PlayerSportsBetLimit> PlayerSportsBetLimits { get; set; }
    }
}
