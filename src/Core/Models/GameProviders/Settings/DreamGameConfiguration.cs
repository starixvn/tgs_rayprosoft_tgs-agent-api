using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record DreamGameConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
        public string AgentName { get; init; } = default!;

        public string PlayerPassword { get; init; } = default!;
    }
}
