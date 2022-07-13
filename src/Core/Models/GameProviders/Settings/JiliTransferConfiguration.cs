using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record JiliTransferConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public string AgentKey { get; init; }
    }
}
