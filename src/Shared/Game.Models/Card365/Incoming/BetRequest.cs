using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Incoming
{
    public record BetRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; init; }

        [JsonPropertyName("userId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("extension3")]
        public string Token { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("gameNumber")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("txId")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("gameId")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int ExternalGameId { get; init; }

        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; init; }

        [JsonPropertyName("betTransTime")]
        public string Timestamp { get; init; }
    }
}