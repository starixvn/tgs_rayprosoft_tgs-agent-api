using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record PlaceBetRequest : TransactionRequest
    {
        [JsonPropertyName("transactionId")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
    }
}