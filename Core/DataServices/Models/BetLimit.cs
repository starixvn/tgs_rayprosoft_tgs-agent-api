using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class BetLimit
    {
        public int Id { get; set; }

        public string CurrencyCode { get; set; }

        public virtual Currency Currency { get; set; }

        public int MinBet { get; set; }

        public int MaxBet { get; set; }

        public int MaxBetPerMatch { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual ICollection<ResellerBetLimit> ResellerBetLimits { get; set; }

        public virtual ICollection<LicenseeBetLimit> LicenseeBetLimits { get; set; }

        public virtual ICollection<BrandBetLimit> BrandBetLimits { get; set; }

        public virtual ICollection<BrandGroupBetLimit> BrandGroupBetLimits { get; set; }
    }
}