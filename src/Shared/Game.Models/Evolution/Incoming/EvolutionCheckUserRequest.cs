using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Shared.Game.Models.Evolution.Incoming
{
    public record EvolutionCheckUserRequest : EvolutionBaseRequest
    {
        [JsonPropertyName("channel")]
        public EvolutionChannel Channel { get; set; }
    }
}
