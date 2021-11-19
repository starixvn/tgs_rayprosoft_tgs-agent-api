using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record BetConstructConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string SharedKey { get; set; }
        public string PartnerId { get; set; }

        public Uri Ua2AuthenticationUrl { get; set; }
    }
}
