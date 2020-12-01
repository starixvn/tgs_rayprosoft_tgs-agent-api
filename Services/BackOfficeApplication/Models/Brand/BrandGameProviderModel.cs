using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOfficeApplication.Models.Brand
{
    public class BrandGameProviderModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Configuration Configuration { get; set; }
    }

    public class Configuration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public Status Status { get; set; }
        public string DesktopEndpoint { get; set; }
        public string MobileEndpoint { get; set; }
        public string ConfigurationData { get; set; }
        public bool IsDefault { get; set; }
    }
}