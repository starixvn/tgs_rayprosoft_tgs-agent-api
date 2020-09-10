using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedLicenseeGame
    {
        public Guid GameProviderId { get; init; }
        public Guid GameId { get; init; }
        public string ExternalGameId { get; init; }
        public PlatformType PlatformType { get; init; }
        public bool SubGame { get; init; }
    }
}