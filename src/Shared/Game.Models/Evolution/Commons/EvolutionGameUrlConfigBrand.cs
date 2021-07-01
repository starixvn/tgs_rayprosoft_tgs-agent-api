using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameUrlConfigBrand
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("skin")]
        public string Skin { get; set; }
    }
}
