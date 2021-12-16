using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Avia
{
    public record AviaLoginResponse
    {
        [JsonPropertyName("Error")]
        public string Error { get; init; }

        [JsonPropertyName("Url")]
        public string Url { get; init; }
    }
}
