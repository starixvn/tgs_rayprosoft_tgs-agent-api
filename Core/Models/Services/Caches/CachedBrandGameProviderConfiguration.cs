using System;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrandGameProviderConfiguration
    {
        public Guid GameProviderId { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
        public string DefaultLanguageCode { get; set; }
        public string BrandLogoUrl { get; set; }
        public QueryString QueryStrings { get; set; }
    }

    public record QueryString
    {
        public string Desktop { get; set; }
        public string Mobile { get; set; }
    }
}