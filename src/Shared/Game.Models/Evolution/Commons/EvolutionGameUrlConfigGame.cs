using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameUrlConfigGame
    {
        [JsonPropertyName("category")]
        public string category { get; set; }
        [JsonPropertyName("interface")]
        public string @interface { get; set; }
        [JsonPropertyName("table")]
        public EvolutionGameUrlConfigGameTable Table { get; set; }
    }
}
