using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionPromoTransaction
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("voucherId")]
        public string VoucherId { get; set; }
        [JsonPropertyName("remainingRounds")]
        public long RemainingRounds { get; set; }
        [JsonPropertyName("jackpots")]
        public EvolutionPromoTransactionJackpots Jackpots { get; set; }
    }
}
