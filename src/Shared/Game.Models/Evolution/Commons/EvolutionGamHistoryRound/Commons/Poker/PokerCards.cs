using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerCards
    {
        [JsonPropertyName("dealer")]
        public string[] Dealer { get; set; }
        [JsonPropertyName("flop")]
        public string[] Flop { get; set; }
        [JsonPropertyName("player")]
        public string[] Player { get; set; }
        [JsonPropertyName("river")]
        public string[] River { get; set; }
    }
}
