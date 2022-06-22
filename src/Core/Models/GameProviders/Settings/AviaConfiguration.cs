using System;
using TGS.Core.Models.GameProviders.Settings.Avia;

namespace TGS.Core.Models.GameProviders.Settings;

public record AviaConfiguration : PluggableConfigurationBase
{
    public decimal AmountMaximum { get; set; }

    public string AmountFormat { get; set; }

    public AviaBrandMerchant[] BrandMerchants { get; set; }
}
