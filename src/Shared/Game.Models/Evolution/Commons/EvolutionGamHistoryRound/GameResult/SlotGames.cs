using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record SlotGames
    {
        [JsonPropertyName("link")]
        public string Path { get; set; }
    }
}
