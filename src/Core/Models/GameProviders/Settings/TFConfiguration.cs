using System;
using TGS.Core.Models.GameProviders.Settings.TF;

namespace TGS.Core.Models.GameProviders.Settings;

public record TFConfiguration : PluggableConfigurationBase
{        
    public TFBrandMerchant[] BrandMerchants { get; set; }
}
