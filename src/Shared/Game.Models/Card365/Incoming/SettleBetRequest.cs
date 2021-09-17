using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Incoming
{
    public record SettleBetRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; init; }

        [JsonPropertyName("userId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("extension3")]
        public string Token { get; init; }

        [JsonPropertyName("agentId")]
        public string AgentId { get; init; }

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

        [JsonPropertyName("realBetMoney")]
        public decimal RealBetMoney { get; init; }

        [JsonPropertyName("payAmount")]
        public decimal PayAmount { get; init; }

        [JsonPropertyName("realPayMoney")]
        public decimal RealPayMoney { get; init; }

        [JsonPropertyName("commission")]
        public decimal Commission { get; init; }

        [JsonPropertyName("validBet")]
        public decimal ValidBet { get; init; }

        [JsonPropertyName("profit")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("betTransTime")]
        public string BetTimestamp { get; init; }

        [JsonPropertyName("payTransTime")]
        public string SettleTimestamp { get; init; }

        [JsonPropertyName("tiles")]
        public string Tiles { get; init; }

        [JsonPropertyName("banker")]
        public int Banker { get; init; }

        [JsonPropertyName("chair")]
        public int Chair { get; init; }

        [JsonPropertyName("playId")]
        public int PlayId { get; init; }

        [JsonPropertyName("outMoney")]
        public decimal OutMoney { get; init; }

        [JsonPropertyName("reward")]
        public decimal Reward { get; init; }
    }
}