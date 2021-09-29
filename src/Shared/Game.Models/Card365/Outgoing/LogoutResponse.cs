using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Outgoing
{
    public record LogoutResponse
    {
        [JsonPropertyName("status")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Status { get; init; }
    }
}