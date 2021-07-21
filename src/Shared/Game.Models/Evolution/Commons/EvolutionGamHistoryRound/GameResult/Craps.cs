using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Craps;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record Craps
    {
        [JsonPropertyName("rolls")]
        public Roll[] Rolls { get; set; }
    }
}
