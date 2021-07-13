using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record InstantRoulette
    {
        [JsonPropertyName("outcomes")]
        public InstantRouletteOutcome[] Outcomes { get; set; }
    }
}
