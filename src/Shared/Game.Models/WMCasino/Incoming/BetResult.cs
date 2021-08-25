using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record BetResult
    {
        [JsonPropertyName("user")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("betId")]
        public string BetId { get; init; }

        [JsonPropertyName("betTime")]
        public string BetTime { get; init; }

        [JsonPropertyName("settime")]
        public string SettleTime { get; init; }

        [JsonPropertyName("bet")]
        public decimal BetAmount { get; init; }

        [JsonPropertyName("validbet")]
        public decimal ValidBet { get; init; }

        [JsonPropertyName("winLoss")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("gid")]
        public string ExternalGameId { get; init; }

        [JsonPropertyName("round")]
        public string RoundId { get; init; }

        [JsonPropertyName("subround")]
        public string SubRoundId { get; init; }

        [JsonPropertyName("betwalletid")]
        public string BetWalletIds { get; init; }

        [JsonPropertyName("resultwalletid")]
        public string ResultWalletIds { get; init; }

        [JsonPropertyName("betCode")]
        public string BetTypeCode { get; init; }

        [JsonPropertyName("betResult")]
        public string BetType { get; init; }

        [JsonPropertyName("gameResult")]
        public string GameResult { get; init; }

        [JsonPropertyName("commission")]
        public string Commission { get; init; }

        [JsonPropertyName("reset")]
        public string Reset { get; init; }
    }
}