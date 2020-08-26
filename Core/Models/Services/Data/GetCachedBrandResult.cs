using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public class GetCachedBrandResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string TimeZoneId { get; set; }
        public Status Status { get; set; }
        public string LoginUrl { get; set; } = string.Empty;
        public string CashierUrl { get; set; } = string.Empty;
        public string HelpUrl { get; set; } = string.Empty;
        public string TermsUrl { get; set; } = string.Empty;
        public string MobileLoginUrl { get; set; } = string.Empty;
        public string MobileCashierUrl { get; set; } = string.Empty;
        public string MobileHelpUrl { get; set; } = string.Empty;
        public string MobileTermsUrl { get; set; } = string.Empty;
        public Guid ResellerId { get; set; }
        public string ResellerCode { get; set; }
    }
}