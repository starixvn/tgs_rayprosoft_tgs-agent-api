using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Shared.Game.Models.Evolution.Incoming
{
    public record EvolutionCancelRequest : EvolutionBaseRequest
    {
        [JsonPropertyName("currency")]
        public string Currency { get; init; }
        [JsonPropertyName("game")]
        public EvolutionGame Game { get; init; }
        [JsonPropertyName("transaction")]
        public EvolutionTransaction Transaction { get; init; }
    }
}
