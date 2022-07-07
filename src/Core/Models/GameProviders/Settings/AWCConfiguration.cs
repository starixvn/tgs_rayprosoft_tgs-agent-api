using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record AWCConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public Uri ApiRecordUrl { get; init; }
    }
}
