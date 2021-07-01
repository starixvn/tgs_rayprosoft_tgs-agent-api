using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.AllBet.Enums;

namespace TGS.Shared.Game.Models.AllBet.Incoming
{
    public record TransactionDetail
    {
        [JsonPropertyName("betNum")]
        public long BetNumber { get; init; }

        [JsonPropertyName("gameRoundId")]
        public long RoundId { get; init; }

        [JsonPropertyName("status")]
        public BetStatus Status { get; init; }

        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; init; }

        [JsonPropertyName("deposit")]
        public decimal Deposit { get; init; }

        [JsonPropertyName("gameType")]
        public int GameType { get; init; }

        [JsonPropertyName("betType")]
        public int BetType { get; init; }

        [JsonPropertyName("commission")]
        public int Commission { get; init; }

        [JsonPropertyName("exchangeRate")]
        public decimal ExchangeRate { get; init; }

        [JsonPropertyName("gameResult")]
        public string GameResult { get; init; }

        [JsonPropertyName("gameResult2")]
        public string GameResultExtended { get; init; }

        [JsonPropertyName("winOrLossAmount")]
        public decimal? WinOrLossAmount { get; init; }

        [JsonPropertyName("validAmount")]
        public decimal? ValidAmount { get; init; }

        [JsonPropertyName("betTime")]
        public string BetTime { get; init; }

        [JsonPropertyName("tableName")]
        public string TableName { get; init; }

        [JsonPropertyName("betMethod")]
        public int BetMethod { get; init; }

        [JsonPropertyName("appType")]
        public int AppType { get; init; }

        [JsonPropertyName("gameRoundStartTime")]
        public string RoundStartTime { get; init; }

        [JsonPropertyName("gameRoundEndTime")]
        public string RoundEndTime { get; init; }

        [JsonPropertyName("ip")]
        public string Ip { get; init; }
    }
}