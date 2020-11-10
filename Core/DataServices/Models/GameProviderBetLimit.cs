using System;

namespace TGS.Core.DataServices.Models
{
    public class GameProviderBetLimit
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual GameProvider GameProvider { get; set; }

        public int BetLimitGroupId { get; set; }

        public virtual BetLimitGroup BetLimitGroup { get; set; }

        public string CurrencyCode { get; set; }

        public virtual Currency Currency { get; set; }
    }
}