using System.Numerics;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Incoming
{
    public record JiliPlaceBetAndSettleResponse : JiliBaseResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("username")]
        public string JiliPlayerId { get; init; }

        [JsonPropertyName("txId")]
        [JsonConverter(typeof(BigIntegerConvertor))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BigInteger? TransactionId { get; init; }
    }
}
