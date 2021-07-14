using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundDataDealer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
    }
}
