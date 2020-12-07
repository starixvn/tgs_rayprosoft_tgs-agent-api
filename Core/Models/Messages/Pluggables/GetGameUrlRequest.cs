// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;

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

        [JsonPropertyName("token")]
        public PlayerTokenData Token { get; init; }

        [JsonPropertyName("language")]
        public string LanguageCode { get; init; }
    }
}