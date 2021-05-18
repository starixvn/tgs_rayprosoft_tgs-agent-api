using System;
using TGS.Core.Models.GameProviders.Settings.JDB;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record JDBConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string ClientDomain { get; init; }
        public JDBBrandMerchant[] BrandMerchants { get; set; }
        public bool MoreGame { get; set; }
    }
}