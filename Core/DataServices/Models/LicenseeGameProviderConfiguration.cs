using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeGameProviderConfiguration
    {
        public Guid Id { get; set; }

        public Guid LicenseeId { get; set; }

        public virtual Licensee Licensee { get; set; }

        public string DefaultLanguageCode { get; set; }

        public virtual Language DefaultLanguage { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual GameProvider GameProvider { get; set; }

        public string LicenseeLogoUrl { get; set; }

        public string DesktopQueryString { get; set; }

        public string MobileQueryString { get; set; }

        public Guid GameProviderConfigurationId { get; set; }

        public virtual GameProviderConfiguration GameProviderConfiguration { get; set; }
    }
}