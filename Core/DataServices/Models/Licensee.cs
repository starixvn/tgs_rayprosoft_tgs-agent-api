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

        public Reseller Reseller { get; set; }

        public string ClientId { get; set; }

        public string SecretKey { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual ICollection<LicenseeGame> Games { get; set; }

        public virtual ICollection<LicenseeCurrency> LicenseeCurrencies { get; set; }

        public virtual ICollection<LicenseeLanguage> LicenseeLanguages { get; set; }

        public virtual ICollection<LicenseeGameProviderConfiguration> LicenseeGameProviderConfigurations { get; set; }

        public virtual ICollection<LicenseeBetLimit> BetLimitGroups { get; set; }

        public virtual ICollection<PlayerLoginRecord> PlayerLoginRecords { get; set; }
    }
}