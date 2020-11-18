using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Licensee : BaseEntity
    {
        public Status Status { get; set; }

        public string Description { get; set; }

        public string TimeZoneId { get; set; }

        public Guid ResellerId { get; set; }

        public virtual Reseller Reseller { get; set; }

        public string ClientId { get; set; }

        public string SecretKey { get; set; }

        public string ApiKey { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual ICollection<LicenseeGame> Games { get; set; }

        public virtual ICollection<LicenseeCurrency> Currencies { get; set; }

        public virtual ICollection<LicenseeLanguage> Languages { get; set; }

        public virtual ICollection<LicenseeGameProvider> GameProviders { get; set; }

        public virtual ICollection<LicenseeGameProviderConfiguration> GameProviderConfigurations { get; set; }

        public virtual ICollection<LicenseeBetLimit> BetLimitGroups { get; set; }

        public virtual ICollection<PlayerLoginRecord> PlayerLoginRecords { get; set; }
    }
}