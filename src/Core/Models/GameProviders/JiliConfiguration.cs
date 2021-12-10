using System;
using System.Collections.Generic;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.Jili;

namespace TGS.Core.Models.GameProviders
{
    public record JiliConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public IEnumerable<JiliMerchant> Merchants { get; init; }
    }
}
