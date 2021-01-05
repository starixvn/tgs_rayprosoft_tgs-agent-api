using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Brand : BaseEntity<Guid>
    {
        public Status Status { get; set; }

        public string Description { get; set; }

        public string OperationalEmail { get; set; }

        public string TimeZoneId { get; set; }

        public Guid LicenseeId { get; set; }

        public virtual Licensee Licensee { get; set; }

        public string ClientId { get; set; }

        public string SecretKey { get; set; }

        public string ApiKey { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual ICollection<BrandGame> Games { get; set; }

        public virtual ICollection<BrandCurrency> Currencies { get; set; }

        public virtual ICollection<BrandLanguage> Languages { get; set; }

        public virtual ICollection<BrandGameProvider> GameProviders { get; set; }

        public virtual ICollection<BrandBetLimit> BetLimits { get; set; }

        public virtual ICollection<BrandGroup> BrandGroups { get; set; }

        public virtual ICollection<PlayerLoginRecord> PlayerLoginRecords { get; set; }
        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<StationLetter> StationLetters { get; set; }
    }
}