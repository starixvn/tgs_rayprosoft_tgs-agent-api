using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public abstract record TransactionRequest : BaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; init; }

        [JsonPropertyName("gameId")]
        public string GameId { get; init; }

        [JsonPropertyName("roundId")]
        public string RoundId { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("reference")]
        public string Reference { get; init; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; init; }

        [JsonPropertyName("roundDetails")]
        public string RoundDetails { get; init; }
    }
}