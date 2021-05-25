using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record PlaceBetRequest : BaseRequest
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

            [JsonPropertyName("matchId")]
            public int MatchId { get; init; }

            [JsonPropertyName("betTime")]
            public string BetTime { get; init; }

            [JsonPropertyName("betAmount")]
            public decimal BetAmount { get; init; }

            [JsonPropertyName("actualAmount")]
            public decimal ActualAmount { get; init; }

            [JsonPropertyName("updateTime")]
            public string UpdateTime { get; init; }

            [JsonPropertyName("tsId")]
            public string Token { get; init; }

            [JsonPropertyName("refId")]
            public long ReferenceId { get; init; }
        }
    }
}