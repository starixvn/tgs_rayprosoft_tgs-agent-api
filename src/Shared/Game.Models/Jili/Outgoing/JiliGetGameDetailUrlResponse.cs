using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Outgoing
{
    public record JiliGetGameDetailUrlResponse
    {
        [JsonPropertyName("ErrorCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("Message")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("Url")]
        public string Url { get; init; }
    }
}
