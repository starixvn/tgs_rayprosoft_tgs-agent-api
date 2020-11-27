using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class GameProviderConfiguration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public Status Status { get; set; }
        public Guid GameProviderId { get; set; }
        public virtual GameProvider GameProvider { get; set; }
        public string DesktopEndpoint { get; set; }
        public string MobileEndpoint { get; set; }
        public string ConfigurationData { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }

        public virtual ICollection<BrandGameProvider> BrandGameProviders { get; set; }
    }
}