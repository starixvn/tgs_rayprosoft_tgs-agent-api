using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedSubGame
    {
        public int Id { get; init; }
        public string ExternalId { get; init; }
        public GamePlatformType PlatformType { get; init; }
    }
}