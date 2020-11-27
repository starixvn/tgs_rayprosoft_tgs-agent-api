using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrand
    {
        public string Code { get; init; }     
        public string Name { get; init; }
        public string TimeZoneId { get; init; }
        public Guid ResellerId { get; init; }
        public string ResellerCode { get; init; }
        public Guid LicenseeId { get; init; }
        public string LicenseeCode { get; init; }
        public Status Status { get; init; }
        public string ClientId {  get; init; }
        public string SecretKey {  get; init; }
    }
}