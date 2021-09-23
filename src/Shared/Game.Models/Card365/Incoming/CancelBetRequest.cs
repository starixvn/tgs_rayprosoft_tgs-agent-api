using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Incoming
{
    public record CancelBetRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; init; }

        [JsonPropertyName("userId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("extension3")]
        public string Token { get; init; }

        [JsonPropertyName("gameNumber")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("updateTime")]
        public string Timestamp { get; init; }
    }
}