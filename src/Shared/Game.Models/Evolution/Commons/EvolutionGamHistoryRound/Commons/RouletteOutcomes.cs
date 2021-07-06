using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons
{
    public record RouletteOutcomes
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("number")]
        public string Number { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
