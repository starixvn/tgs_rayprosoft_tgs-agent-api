using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedPlayerLoginRecord
    {
        public string PlayerCode { get; init; }
        public PlatformType PlatformType { get; init; }
        public DateTimeOffset ExpiredOn { get; init; }
    }
}
