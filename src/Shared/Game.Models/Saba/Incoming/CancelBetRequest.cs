using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record CancelBetRequest : BaseRequest
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
            [JsonPropertyName("refId")]
            public string ExternalRoundId { get; init; }
        }
    }
}