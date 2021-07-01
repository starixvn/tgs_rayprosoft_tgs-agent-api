using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Shared.Game.Models.Evolution.Outgoing
{
    public record EvolutionGameUrlRequest
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        [JsonPropertyName("player")]
        public EvolutionGameUrlPlayer Player { get; set; }
        [JsonPropertyName("config")]
        public EvolutionGameUrlConfig Config { get; set; }
    }
}
