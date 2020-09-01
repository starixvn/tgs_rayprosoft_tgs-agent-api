using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Brand : BaseEntity
    {
        public Status Status { get; set; }

        public string Description { get; set; }

        public string TimeZoneId { get; set; }

        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }

        public string LoginUrl { get; set; }

        public string CashierUrl { get; set; }

        public string HelpUrl { get; set; }

        public string TermsUrl { get; set; }

        public string MobileLoginUrl { get; set; }

        public string MobileCashierUrl { get; set; }

        public string MobileHelpUrl { get; set; }

        public string MobileTermsUrl { get; set; }

        public virtual ICollection<BrandGame> Games { get; set; }

        public virtual ICollection<BrandCurrency> BrandCurrencies { get; set; }

        public virtual ICollection<BrandLanguage> BrandLanguages { get; set; }

        public virtual ICollection<BrandGameProviderConfiguration> BrandGameProviderConfigurations { get; set; }

        public virtual ICollection<BrandBetLimit> BetLimitGroups { get; set; }
    }
}