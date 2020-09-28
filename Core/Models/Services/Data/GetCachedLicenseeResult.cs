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
        public Guid ResellerId { get; init; }
        public string ResellerCode { get; init; }   
        public string ClientId {  get; init; }
        public string SecretKey { get; init; }
    }
}