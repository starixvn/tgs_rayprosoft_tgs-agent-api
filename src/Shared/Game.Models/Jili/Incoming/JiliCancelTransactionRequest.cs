using System.Numerics;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Incoming
{
    public record JiliCancelTransactionRequest
    {
        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("game")]
        public int ExternalGameId { get; init; }

        [JsonPropertyName("round")]
        [JsonConverter(typeof(BigIntegerConvertor))]
        public BigInteger ExternalRoundId { get; init; }

        [JsonPropertyName("userId")]
        public string JiliPlayerId { get; init; }

        [JsonPropertyName("reqId")]
        public string RequestId { get; init; }

        [JsonPropertyName("token")]
        public string Token { get; init; }

        [JsonPropertyName("winloseAmount")]
        public decimal WinloseAmount { get; init; }
    }
}
