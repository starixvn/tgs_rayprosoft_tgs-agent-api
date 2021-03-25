using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrandGame
    {
        public Guid GameProviderId { get; init; }
        public int GameId { get; init; }
        public string ExternalGameId { get; init; }
        public GamePlatformType PlatformType { get; init; }
        public GameStatus BrandGameStatus { get; init; }
        public GameTag Tag { get; init; }
    }
}