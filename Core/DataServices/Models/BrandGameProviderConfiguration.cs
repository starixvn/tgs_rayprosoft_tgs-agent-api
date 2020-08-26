using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandGameProviderConfiguration
    {
        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public Brand Brand { get; set; }
        
        public string DefaultLanguageCode { get; set; }

        public Language DefaultLanguage { get; set; }

        public Guid GameProviderId { get; set; }

        public GameProvider GameProvider { get; set; }

        public string BrandLogoUrl { get; set; }

        public string DesktopQueryString { get; set; }

        public string MobileQueryString { get; set; }

        public Guid GameProviderConfigurationId { get; set; }

        public GameProviderConfiguration GameProviderConfiguration { get; set; }
    }
}
