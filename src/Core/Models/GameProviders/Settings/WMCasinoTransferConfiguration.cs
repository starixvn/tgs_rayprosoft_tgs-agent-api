using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record WMCasinoTransferConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
    }
}
