using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundParticipantHandDecision
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
