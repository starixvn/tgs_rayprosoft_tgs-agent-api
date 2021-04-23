using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record PragmaticPlayConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string SecureLogin { get; init; }
        public string SecretKey { get; init; }
        public string ProviderId { get; init; }
        public string GameUrlQueryStringTemplate { get; init; }
        public int DelaySecondsForPlaceBet { get; init; }
    }
}