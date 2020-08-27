using System;

namespace TGS.Core.Models.Services.Data
{
    public class GetCachedBrandGameProviderConfigurationResult
    {
        public Guid GameProviderConfigurationId { get; internal set; }
        public string DefaultLanguageCode { get; internal set; }
        public string BrandLogoUrl { get; internal set; }
        public string DesktopQueryString { get; internal set; }
        public string MobileQueryString { get; internal set; }
    }
}