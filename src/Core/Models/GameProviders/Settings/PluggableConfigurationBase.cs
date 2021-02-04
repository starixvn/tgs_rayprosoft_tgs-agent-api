using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record PluggableConfigurationBase
    {
        public Uri ServiceEndpointUrl { get; set; }
    }
}