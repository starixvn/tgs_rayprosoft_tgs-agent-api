using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record UnsettleBetRequest : BaseRequest
    {
        [JsonPropertyName("message")]
        public Data Body { get; init; }

        public record Data
        {
            [JsonPropertyName("action")]
            public string Action { get; init; }

            [JsonPropertyName("txns")]
            public Transaction[] Transactions { get; init; }
        }

        public record Transaction
        {
            [JsonPropertyName("userId")]
            public string UserId { get; init; }

            [JsonPropertyName("txId")]
            public long ExternalTransactionId { get; init; }

            [JsonPropertyName("updateTime")]
            public string UpdateTime { get; init; }
        }
    }
}