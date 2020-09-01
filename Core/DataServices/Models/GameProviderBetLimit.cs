using System;

namespace TGS.Core.DataServices.Models
{
    public class GameProviderBetLimit
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public Guid GameProviderId { get; set; }

        public GameProvider GameProvider { get; set; }

        public int BetLimitGroupId { get; set; }

        public BetLimitGroup BetLimitGroup { get; set; }

        public string CurrencyCode { get; set; }

        public Currency Currency { get; set; }
    }
}