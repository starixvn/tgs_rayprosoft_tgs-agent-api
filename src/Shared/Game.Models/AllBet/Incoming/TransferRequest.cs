using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.AllBet.Enums;

namespace TGS.Shared.Game.Models.AllBet.Incoming
{
    public record TransferRequest
    {
        [JsonPropertyName("tranId")]
        public long TransactionId { get; init; }

        [JsonPropertyName("player")]
        public string PlayerId { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("reason")]
        public string Reason { get; init; }

        [JsonPropertyName("type")]
        public TransferType Type { get; init; }

        [JsonPropertyName("isRetry")]
        public bool IsRetry { get; init; }

        [JsonPropertyName("details")]
        public TransactionDetail[] Details { get; init; }
    }
}