using System;

namespace TGS.Core.Models.Services.Data
{
    public class GetCachedLicenseeGameProviderConfigurationResult
    {
        public Guid GameProviderConfigurationId { get; set; }
        public string DefaultLanguageCode { get; set; }
        public string LicenseeLogoUrl { get; set; }
        public string DesktopQueryString { get; set; }
        public string MobileQueryString { get; set; }
    }
}