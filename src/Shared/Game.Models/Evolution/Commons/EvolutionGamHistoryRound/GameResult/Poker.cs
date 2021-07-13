using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record Poker
    {
        [JsonPropertyName("bonus")]
        public PokerBonus Bonus { get; set; }
        [JsonPropertyName("cards")]
        public PokerCards Cards { get; set; }
        [JsonPropertyName("dealer")]
        public PokerDealer Dealer { get; set; }
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; }
        [JsonPropertyName("player")]
        public PokerPlayer Player { get; set; }
    }
}
