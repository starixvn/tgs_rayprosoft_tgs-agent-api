using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Shared.Game.Models.Evolution.Incoming
{
    public record EvolutionBalanceRequest : EvolutionBaseRequest
    {
        [JsonPropertyName("game")]
        public EvolutionGame Game { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
}
