using System;

namespace TGS.Core.DataServices.Models
{
    public class Brand : BaseEntity
    {
        public string Description { get; set; }

        public string OperationalEmail { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string TimeZoneId { get; set; }

        public bool IpChecking { get; set; }

        public bool SingleSession { get; set; }

        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }

        public Guid LicenseeId { get; set; }

        public Licensee Licensee { get; set; }

        public string LoginUrl { get; set; }

        public string CashierUrl { get; set; }

        public string HelpUrl { get; set; }

        public string TermsUrl { get; set; }

        public string MobileLoginUrl { get; set; }

        public string MobileCashierUrl { get; set; }

        public string MobileHelpUrl { get; set; }

        public string MobileTermsUrl { get; set; }
    }
}