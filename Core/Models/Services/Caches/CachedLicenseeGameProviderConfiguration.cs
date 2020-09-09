using System;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedLicenseeGameProviderConfiguration
    {
        public Guid GameProviderId { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public string DefaultLanguageCode { get; init; }
        public string LicenseeLogoUrl { get; init; }
        public QueryString QueryStrings { get; init; }
    }

    public record QueryString
    {
        public string Desktop { get; init; }
        public string Mobile { get; init; }
    }
}