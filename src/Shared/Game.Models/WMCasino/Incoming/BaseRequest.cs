using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record BaseRequest
    {
        [JsonPropertyName("cmd")]
        public string Action { get; init; }

        [JsonPropertyName("requestDate")]
        public string Timestamp { get; init; }

        [JsonPropertyName("signature")]
        public string Signature { get; init; }
    }
}