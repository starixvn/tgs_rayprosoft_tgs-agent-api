using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerBonus
    {
        [JsonPropertyName("tripsBonus")]
        public PokerBonusTripsbonus TripsBonus { get; set; }
    }
}
