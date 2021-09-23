using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Outgoing
{
    public record GetKeyResponse
    {
        [JsonPropertyName("status")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Status { get; init; }

        [JsonPropertyName("key")]
        public string Key { get; init; }
    }
}