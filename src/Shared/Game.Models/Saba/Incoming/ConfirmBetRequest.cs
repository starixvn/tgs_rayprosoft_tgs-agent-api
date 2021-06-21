using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record ConfirmBetRequest : BaseRequest
    {
        [JsonPropertyName("message")]
        public Data Body { get; init; }

        public record Data
        {
            [JsonPropertyName("action")]
            public string Action { get; init; }

            [JsonPropertyName("userId")]
            public string UserId { get; init; }

            [JsonPropertyName("updateTime")]
            public string UpdateTime { get; init; }

            [JsonPropertyName("txns")]
            public Transaction[] Transactions { get; init; }
        }

        public record Transaction
        {
            [JsonPropertyName("txId")]
            public long ExternalTransactionId { get; init; }

            [JsonPropertyName("licenseeTxId")]
            public long ExternalRoundId { get; init; }

            [JsonPropertyName("actualAmount")]
            public decimal ActualAmount { get; init; }

            [JsonPropertyName("odds")]
            public decimal Odds { get; init; }

            [JsonPropertyName("oddsType")]
            public short OddsType { get; init; }

            [JsonPropertyName("isOddsChanged")]
            public bool IsOddsChanged { get; init; }
        }
    }
}