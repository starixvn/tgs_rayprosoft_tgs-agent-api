using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class BetLimitGroup
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual ICollection<GameProviderBetLimit> GameProviderBetLimits { get; set; }

        public virtual ICollection<BrandBetLimit> Brands { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }
    }
}
