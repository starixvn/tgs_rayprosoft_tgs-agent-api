using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrandGameProviderDetail
    {
        public Guid Id { get; init; }
        public string Code { get; init; }
        public Status Status { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public GameType[] GameTypes { get; init; }
    }
}
