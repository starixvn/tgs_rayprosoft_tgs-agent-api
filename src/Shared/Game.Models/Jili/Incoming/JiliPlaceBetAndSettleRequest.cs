using System.Numerics;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Incoming;

public record JiliPlaceBetAndSettleRequest
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

    [JsonPropertyName("isFreeRound")]
    public bool? IsFreeRound { get; init; }

    [JsonPropertyName("userId")]
    public string JiliPlayerId { get; init; }

    [JsonPropertyName("reqId")]
    public string RequestId { get; init; }

    [JsonPropertyName("token")]
    public string Token { get; init; }

    [JsonPropertyName("wagersTime")]
    public long WagersTime { get; init; }

    [JsonPropertyName("winloseAmount")]
    public decimal WinloseAmount { get; init; }

    [JsonPropertyName("transactionId")]
    [JsonConverter(typeof(BigIntegerConvertor))]
    public BigInteger? ReferencedExternalRoundId { get; init; }
}
