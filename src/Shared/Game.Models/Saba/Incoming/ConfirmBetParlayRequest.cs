using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record ConfirmBetParlayRequest : BaseRequest
    {
        [JsonPropertyName("message")]
        public Data Body { get; init; }

        public record Data
        {
            [JsonPropertyName("action")]
            public string Action { get; init; }

            [JsonPropertyName("userId")]
            public string UserId { get; init; }

            [JsonPropertyName("currency")]
            public int CurrencyCode { get; init; }

            [JsonPropertyName("updateTime")]
            public string UpdateTime { get; init; }

            [JsonPropertyName("txns")]
            public Transaction[] Transactions { get; init; }

            [JsonPropertyName("ticketDetail")]
            public JsonDocument TicketDetail { get; init; }
        }

        public record Transaction
        {
            [JsonPropertyName("refId")]
            public string ReferenceId { get; init; }

            [JsonPropertyName("txId")]
            public long ExternalTransactionId { get; init; }

            [JsonPropertyName("licenseeTxId")]
            public string ExternalRoundId { get; init; }

            [JsonPropertyName("actualAmount")]
            public decimal ActualAmount { get; init; }

            [JsonPropertyName("isOddsChanged")]
            public bool IsOddsChanged { get; init; }
        }
    }
}