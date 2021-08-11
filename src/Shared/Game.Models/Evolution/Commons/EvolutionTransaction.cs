using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record  EvolutionTransaction
    {
        [JsonPropertyName("id")]
        public string ExternalTransactionId { get; set; }
        [JsonPropertyName("refId")]
        public string ExternalGameActionGroupId { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
