using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Outgoing
{
    public record LoginRequest
    {
        [JsonPropertyName("player")]
        public string PlayerId { get; init; }

        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}