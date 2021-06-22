using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record AllBetConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
    }
}