// ReSharper disable InconsistentNaming

using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record IMESportsConfiguration : PluggableConfigurationBase
    {
        public IMESportsBrandMerchant[] BrandMerchants { get; init; }
        public int ProductWallet { get; init; }
        public bool SupportHttps { get; init; } = false;
    }

    public record IMESportsBrandMerchant
    {
        public string BrandCode { get; init; }
        public string MerchantCode { get; init; }
        public string Prefix { get; init; }
        public Uri ApiUrl { get; init; }
    }
}