using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.BlackJack
{
    public record Decision
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
