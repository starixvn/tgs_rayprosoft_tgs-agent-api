using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameUrlConfig
    {
        [JsonPropertyName("brand")]
        public EvolutionGameUrlConfigBrand Brand { get; set; }
        [JsonPropertyName("game")]
        public EvolutionGameUrlConfigGame Game { get; set; }
        [JsonPropertyName("channel")]
        public EvolutionGameUrlConfigChannel Channel { get; set; }
        [JsonPropertyName("urls")]
        public EvolutionGameUrlConfigUrls Urls { get; set; }
        [JsonPropertyName("freeGames")]
        public bool FreeGames { get; set; }
    }
}
