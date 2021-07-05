using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record  EvolutionTransaction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("refId")]
        public string RefId { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
