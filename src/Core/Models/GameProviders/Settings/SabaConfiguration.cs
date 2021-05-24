using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record SabaConfiguration : PluggableConfigurationBase
    {
        public string OperatorId { get; init; }

        public string VendorId { get; init; }

        public string SecretKey { get; init; }

        public Uri ApiUrl { get; init; }

        public string[] TestEnvironments { get; init; } = Array.Empty<string>();
    }
}