using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionChannel
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
