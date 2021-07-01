using System;
using TGS.Core.Models.GameProviders.Settings.AllBet;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record AllBetConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public AllBetBrandMerchant[] BrandMerchants { get; init; }
    }
}