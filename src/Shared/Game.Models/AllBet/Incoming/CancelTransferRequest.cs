using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Incoming
{
    public record CancelTransferRequest
    {
        [JsonPropertyName("tranId")]
        public int TransactionId { get; init; }

        [JsonPropertyName("originalTranId")]
        public int ReferencedTransactionId { get; init; }

        [JsonPropertyName("reason")]
        public string Reason { get; init; }

        [JsonPropertyName("isRetry")]
        public bool IsRetry { get; init; }
    }
}