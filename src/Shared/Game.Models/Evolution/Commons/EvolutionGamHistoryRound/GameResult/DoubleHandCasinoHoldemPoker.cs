using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.DoubleHandCasinoHoldemPoker;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record DoubleHandCasinoHoldemPoker
    {
        [JsonPropertyName("dealer")]
        public PokerDealer Dealer { get; set; }
        [JsonPropertyName("cards")]
        public PokerCards Cards { get; set; }
        [JsonPropertyName("firstHand")]
        public DoubleHandCasinoHoldemPokerHand FirstHand { get; set; }
        [JsonPropertyName("secondHand")]
        public DoubleHandCasinoHoldemPokerHand SecondHand { get; set; }
    }
}
