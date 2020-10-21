using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Incoming
{
    public record PlaceBetRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; set; }

        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}