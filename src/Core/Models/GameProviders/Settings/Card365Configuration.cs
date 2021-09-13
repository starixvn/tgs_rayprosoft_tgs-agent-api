using System;
using TGS.Core.Models.GameProviders.Settings.Card365;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record Card365Configuration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public string Cert { get; init; }

        public Card365BrandAgent[] BrandAgents { get; init; }
    }
}