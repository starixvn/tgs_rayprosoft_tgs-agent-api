using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedGameProviderConfiguration
    {
        public Status Status { get; init; }
        public string SecretKey { get; init; }
        public Guid GameProviderId { get; init; }
        public string DesktopEndpoint { get; init; }
        public string MobileEndpoint { get; init; }
        public string ConfigurationData { get; init; }
        public bool IsDefault { get; init; }
    }
}