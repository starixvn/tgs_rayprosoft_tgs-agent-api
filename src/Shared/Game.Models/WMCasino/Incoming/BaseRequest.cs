using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record BaseRequest
    {
        [JsonPropertyName("cmd")]
        public string Action { get; set; }

        [JsonPropertyName("requestDate")]
        public string Timestamp { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}