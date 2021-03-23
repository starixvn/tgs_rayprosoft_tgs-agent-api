using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GetPlayerLoginRecordResult
    {
        public Guid BrandId { get; init; }
        public PlatformType PlatformType { get; init; }
        public LoginStatus Status { get; init; }
        public DateTimeOffset CreatedOn { get; init; }
        public DateTimeOffset ExpiredOn { get; init; }
    }
}