using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons
{
    public record Card
    {
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}
