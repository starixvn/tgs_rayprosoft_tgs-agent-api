using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrandGameProvider
    {
        public Guid GameProviderId { get; init;  }
        public string GameProviderCode { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public Status BrandGameProviderStatus { get; init; }
    }
}
