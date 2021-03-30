using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record PlaceBetResponse : BalanceResponse
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("usedPromo")]
        public decimal UsedPromo { get; init; }
    }
}