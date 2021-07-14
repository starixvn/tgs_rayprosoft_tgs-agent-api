using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerBonusTripsbonus
    {
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
        [JsonPropertyName("handType")]
        public string HandType { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
