using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionBaseRequest
    {
        [JsonPropertyName("sid")]
        public string Sid { get; set; }
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
    }
}
