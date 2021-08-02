using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameTable
    {
        [JsonPropertyName("id")]
        public string ExternalGameId { get; set; }
        [JsonPropertyName("vid")]
        public string Vid { get; set; }
    }
}
