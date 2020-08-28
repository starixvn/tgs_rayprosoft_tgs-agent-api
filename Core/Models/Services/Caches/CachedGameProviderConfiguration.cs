using System;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedGameProviderConfiguration
    {
        public string Description { get; set; }
        public string SecretKey { get; set; }
        public Guid GameProviderId { get; set; }
        public string DesktopEndpoint { get; set; }
        public string MobileEndpoint { get; set; }
        public string ConfigurationData { get; set; }
        public bool IsDefault { get; set; }
    }
}