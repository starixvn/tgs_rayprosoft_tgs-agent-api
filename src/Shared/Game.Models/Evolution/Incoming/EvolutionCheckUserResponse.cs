using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Shared.Game.Models.Evolution.Incoming
{
    public record EvolutionCheckUserResponse : EvolutionBaseResponse
    {
        [JsonPropertyName("sid")]
        public string Sid { get; set; }

    }
}
