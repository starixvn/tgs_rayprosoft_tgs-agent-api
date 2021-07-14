using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record DragonTiger
    {
        [JsonPropertyName("dragon")]
        public DragonTigerCard Dragon { get; set; }
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; }
        [JsonPropertyName("tiger")]
        public DragonTigerCard Tiger { get; set; }
    }
}
