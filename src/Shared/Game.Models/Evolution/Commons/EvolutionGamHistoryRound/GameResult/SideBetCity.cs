using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.SideBetCity;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record SideBetCity
    {
        [JsonPropertyName("cardsDealt")]
        public string[] CardsDealt { get; set; }
        [JsonPropertyName("outcome")]
        public SideBetCityOutcome Outcome { get; set; }
    }
}
