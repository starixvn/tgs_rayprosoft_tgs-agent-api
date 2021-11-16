using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public record BaseRequest
    {
        [JsonPropertyName("AuthToken")]
        public string AuthToken { get; init; }
        
        [JsonPropertyName("TS")]
        public long Timestamp { get; init; }

        [JsonPropertyName("Hash")]
        public string Hash { get; init; }
    }
}
