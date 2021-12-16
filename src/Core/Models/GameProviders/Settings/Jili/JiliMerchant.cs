using System.Collections.Generic;

namespace TGS.Core.Models.GameProviders.Settings.Jili
{
    public record JiliMerchant
    {
        public IEnumerable<string> Brands { get; init; }

        public string AgentId { get; init; }

        public string AgentKey { get; init; }
    }
}
