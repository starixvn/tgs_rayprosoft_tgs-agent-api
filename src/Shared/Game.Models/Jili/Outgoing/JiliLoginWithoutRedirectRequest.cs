// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Outgoing
{
    public record JiliLoginWithoutRedirectRequest
    {
        [JsonPropertyName("Token")]
        public string Token { get; init; }

        [JsonPropertyName("GameId")]
        public string GameExternalId { get; init; }

        [JsonPropertyName("Lang")]
        public string Language { get; init; }

        [JsonPropertyName("isJPEnabled")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
        public int JackpotEnable { get; init; }
    }
}
