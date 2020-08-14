using System;

namespace TGS.Core.DataServices.Models
{
    public class GameProviderConfiguration : BaseEntity
    {
        public string Description { get; set; }
        public string SecretKey { get; set; }
        public Guid GameProviderId { get; set; }
        public GameProvider GameProvider { get; set; }
        public string DesktopEndpoint { get; set; }
        public string MobileEndpoint { get; set; }
        public string ConfigurationData { get; set; }
    }
}