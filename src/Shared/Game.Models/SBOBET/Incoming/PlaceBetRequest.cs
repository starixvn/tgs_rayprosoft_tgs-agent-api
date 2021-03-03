using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record PlaceBetRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
    }
}