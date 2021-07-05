using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Shared.Game.Models.Evolution.Incoming
{
    public record EvolutionDebitRequest : EvolutionBaseRequest
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        [JsonPropertyName("game")]
        public EvolutionGame Game { get; set; }
        [JsonPropertyName("transaction")]
        public EvolutionTransaction Transaction { get; set; }
    }
}
