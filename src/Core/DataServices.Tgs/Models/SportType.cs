using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class SportType
    {
        public Guid Id { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual GameProvider GameProvider { get; set; }

        public string Code { get; set; }

        public string ExternalId { get; set; }

        public virtual ICollection<SportMarketType> MarketTypes { get; set; }
    }
}