using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public class GetCachedGameProviderConfigurationResult
    {
        public Status Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SecretKey { get; set; }
        public Guid GameProviderId { get; set; }
        public string DesktopEndpoint { get; set; }
        public string MobileEndpoint { get; set; }
        public string ConfigurationData { get; set; }
        public bool IsDefault { get; set; }
    }
}