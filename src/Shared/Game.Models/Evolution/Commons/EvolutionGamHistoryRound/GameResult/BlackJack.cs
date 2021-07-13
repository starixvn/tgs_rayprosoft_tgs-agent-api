using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.BlackJack;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record BlackJack
    {
        [JsonPropertyName("dealer")]
        public Dealer Dealer { get; set; }
        [JsonPropertyName("seats")]
        public Seats Seats { get; set; }
        [JsonPropertyName("burnedCards")]
        public object[] BurnedCards { get; set; }
    }
}
