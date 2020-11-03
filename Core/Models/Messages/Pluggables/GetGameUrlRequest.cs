using System.Text.Json.Serialization;
using TGS.Core.Models.Services.Caches;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record GetGameUrlRequest
    {
        [JsonPropertyName("gameprovider")]
        public CachedGameProvider GameProvider { get; init; }

        [JsonPropertyName("game")]
        public CachedGame Game { get; init; }

        [JsonPropertyName("player")]
        public CachedPlayer Player { get; init; }
    }
}
