using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class GameProviderBetLimit
    {
        public Guid Id { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual GameProvider GameProvider { get; set; }

        public string CurrencyCode { get; set; }

        public virtual Currency Currency { get; set; }

        public int MinBet { get; set; }

        public int MaxBet { get; set; }

        public int? MaxBetPerMatch { get; set; }

        public string ExternalSetting { get; set; }

        public virtual ICollection<BrandGroupGameProviderBetLimit> BrandGroupBetLimits { get; set; }
    }
}