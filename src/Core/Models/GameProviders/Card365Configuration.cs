using System;
using TGS.Core.Models.GameProviders.Settings;

namespace TGS.Core.Models.GameProviders
{
    public record Card365Configuration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
    }
}