using System;
using System.Collections.Generic;
using TGS.Core.Models.GameProviders.Settings.Joker;

namespace TGS.Core.Models.GameProviders.Settings;

public record JokerConfiguration : PluggableConfigurationBase
{
    public Uri ApiUrl { get; init; } = default!;

    public Uri GameLaunchUrl { get; init; } = default!;

    public IEnumerable<JokerBrandMerchant> BrandMerchants { get; init; } = default!;
}
