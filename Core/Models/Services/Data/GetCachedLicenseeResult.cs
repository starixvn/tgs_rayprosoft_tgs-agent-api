using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GetCachedLicenseeResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; }
        public string Name { get; init; }
        public string TimeZoneId { get; init; }
        public Status Status { get; init; }
        public string LoginUrl { get; init; } = string.Empty;
        public string CashierUrl { get; init; } = string.Empty;
        public string HelpUrl { get; init; } = string.Empty;
        public string TermsUrl { get; init; } = string.Empty;
        public string MobileLoginUrl { get; init; } = string.Empty;
        public string MobileCashierUrl { get; init; } = string.Empty;
        public string MobileHelpUrl { get; init; } = string.Empty;
        public string MobileTermsUrl { get; init; } = string.Empty;
        public Guid ResellerId { get; init; }
        public string ResellerCode { get; init; }        
    }
}