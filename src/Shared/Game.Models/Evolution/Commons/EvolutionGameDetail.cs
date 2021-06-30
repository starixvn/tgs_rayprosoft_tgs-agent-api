using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameDetail
    {
        [JsonPropertyName("table")]
        public EvolutionGameTable GameTable { get; set; }
    }
}
