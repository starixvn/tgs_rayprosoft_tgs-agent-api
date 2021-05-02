using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record JDBConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string ClientDomain { get; init; }
        public string Parent { get; set; }
    }
}