using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigGameTable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
