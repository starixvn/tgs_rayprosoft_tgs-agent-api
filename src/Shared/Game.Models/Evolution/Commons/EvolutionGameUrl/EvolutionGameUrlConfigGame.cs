using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigGame
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonPropertyName("interface")]
        public string @Interface { get; set; }
        [JsonPropertyName("table")]
        public EvolutionGameUrlConfigGameTable Table { get; set; }
    }
}
