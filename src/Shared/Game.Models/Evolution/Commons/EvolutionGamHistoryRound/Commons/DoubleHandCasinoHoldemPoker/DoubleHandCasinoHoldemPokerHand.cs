using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.DoubleHandCasinoHoldemPoker
{
    public record DoubleHandCasinoHoldemPokerHand
    {
        [JsonPropertyName("player")]
        public PokerPlayer Player { get; set; }
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; }
    }
}
