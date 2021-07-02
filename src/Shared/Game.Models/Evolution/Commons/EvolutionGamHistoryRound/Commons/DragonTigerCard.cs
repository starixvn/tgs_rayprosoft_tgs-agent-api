using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons
{
    public record DragonTigerCard
    {
        [JsonPropertyName("card")]
        public string Card { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}
