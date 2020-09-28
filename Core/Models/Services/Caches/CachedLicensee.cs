using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedLicensee
    {
        public string Code { get; init; }     
        public string Name { get; init; }
        public string TimeZoneId { get; init; }
        public Guid ResellerId { get; init; }
        public string ResellerCode { get; init; }
        public Status Status { get; init; }
        public string ClientId {  get; init; }
        public string SecretKey {  get; init; }
    }
}