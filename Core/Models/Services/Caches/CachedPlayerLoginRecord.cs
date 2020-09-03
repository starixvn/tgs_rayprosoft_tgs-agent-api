using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedPlayerLoginRecord
    {
        public PlatformType PlatformType { get; set; }
        public DateTimeOffset ExpiredOn { get; set; }
    }
}
