using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Tokens
{
    public record PlayerTokenData
    {
        public string PlayerCode { get; init; }
        public Guid LoginRecordId { get; init; }
        public PlatformType PlatformType { get; init; }
        public DateTimeOffset ExpiredOn { get; init; }
    }
}