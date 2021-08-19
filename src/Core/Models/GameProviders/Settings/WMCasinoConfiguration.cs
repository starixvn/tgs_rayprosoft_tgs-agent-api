using System;
using TGS.Core.Models.GameProviders.Settings.WMCasino;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record WMCasinoConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public WMCasinoBrandMerchant[] BrandMerchants { get; init; }

        public int MaxResultCount { get; init; }
    }
}