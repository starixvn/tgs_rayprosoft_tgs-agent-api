using System;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrandGameProviderConfiguration
    {
        public Guid GameProviderId { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public string DefaultLanguageCode { get; init; }
        public QueryString QueryStrings { get; init; }
    }

    public record QueryString
    {
        public string Desktop { get; init; }
        public string Mobile { get; init; }
    }
}