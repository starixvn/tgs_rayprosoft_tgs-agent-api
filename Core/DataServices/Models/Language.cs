using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class Language
    {
        public string CultureCode { get; set; }

        public string LanguageCode { get; set; }

        public string Name { get; set; }

        public string LocalizedName { get; set; }

        public virtual ICollection<BrandLanguage> BrandLanguages { get; set; }

        public virtual ICollection<LicenseeLanguage> LicenseeLanguages { get; set; }

        public virtual ICollection<ResellerLanguage> ResellerLanguages { get; set; }

        public virtual ICollection<GameProviderLanguage> GameProviderLanguages { get; set; }

        public virtual ICollection<BrandGameProvider> BrandGameProviders { get; set; }

        public virtual ICollection<LocalizedString> LocalizedStrings { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }
    }
}