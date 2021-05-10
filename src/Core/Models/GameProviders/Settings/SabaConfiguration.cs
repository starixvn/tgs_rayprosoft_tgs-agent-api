using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record SabaConfiguration : PluggableConfigurationBase
    {
        public string VendorId { get; init; }

        public string SecretKey { get; init; }

        public Uri ApiUrl { get; init; }
    }
}