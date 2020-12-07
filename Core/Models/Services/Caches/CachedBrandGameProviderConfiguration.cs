using System;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrandGameProviderConfiguration
    {
        public Guid GameProviderId { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public string DefaultLanguageCode { get; init; }
        public QueryStrings QueryStrings { get; init; }
    }

    public record QueryStrings
    {
        public string Desktop { get; init; }
        public string Mobile { get; init; }
    }
}