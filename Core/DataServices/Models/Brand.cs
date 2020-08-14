using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Brand : BaseEntity
    {
        public Status Status { get; set; }

        public string Description { get; set; }

        public string OperationalEmail { get; set; }

        public string TimeZoneId { get; set; }

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