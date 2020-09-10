using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedLicensee
    {
        public string Code { get; init; }        
        public string TimeZoneId { get; init; }
        public Guid ResellerId { get; init; }
        public string ResellerCode { get; init; }
        public Status Status { get; init; }
        public LicenseeUrls Urls { get; init; }
    }

    public record LicenseeUrls
    {
        public string Login { get; init; }
        public string Cashier { get; init; }
        public string Help { get; init; }
        public string Terms { get; init; }
        public string MobileCashier { get; init; }
        public string MobileHelp { get; init; }
        public string MobileLogin { get; init; }
        public string MobileTerms { get; init; }
    }
}