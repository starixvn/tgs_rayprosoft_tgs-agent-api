using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record PlaceBetParlayRequest : BaseRequest
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

            [JsonPropertyName("betTime")]
            public string BetTime { get; init; }

            [JsonPropertyName("updateTime")]
            public string UpdateTime { get; init; }

            [JsonPropertyName("totalBetAmount")]
            public decimal TotalBetAmount { get; init; }

            [JsonPropertyName("beforeDiscountAmount")]
            public decimal BeforeDiscountAmount { get; init; }

            [JsonPropertyName("percentage")]
            public decimal Percentage { get; init; }

            [JsonPropertyName("tsId")]
            public string Token { get; init; }

            [JsonPropertyName("txns")]
            public Transaction[] Transactions { get; init; }

            [JsonPropertyName("ticketDetail")]
            public JsonDocument TicketDetail { get; init; }
        }

        public record Transaction
        {
            [JsonPropertyName("refId")]
            public long ReferenceId { get; init; }

            [JsonPropertyName("parlayType")]
            public string ParlayType { get; init; }

            [JsonPropertyName("betAmount")]
            public decimal BetAmount { get; init; }

            [JsonPropertyName("detail")]
            public JsonDocument Detail { get; init; }
        }
    }
}