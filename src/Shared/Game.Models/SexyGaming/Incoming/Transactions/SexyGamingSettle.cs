using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Enums;

namespace TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions
{
    public record SexyGamingSettle
    {
        [JsonPropertyName("platformTxId")]
        public string PlatformTxId { get; init; }
        [JsonPropertyName("userId")]
        public string UserId { get; init; }
        [JsonPropertyName("platform")]
        public string Platform { get; init; }
        [JsonPropertyName("gameType")]
        public string GameType { get; init; }
        [JsonPropertyName("gameCode")]
        public string GameCode { get; init; }
        [JsonPropertyName("gameName")]
        public string GameName { get; init; }
        [JsonPropertyName("betType")]
        public string BetType { get; init; }
        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; init; }
        [JsonPropertyName("winAmount")]
        public decimal WinAmount { get; init; }
        [JsonPropertyName("turnover")]
        public decimal Turnover { get; init; }
        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; init; }
        [JsonPropertyName("updateTime")]
        public DateTimeOffset UpdateTime { get; init; }
        [JsonPropertyName("roundId")]
        public decimal RoundId { get; init; }
        [JsonPropertyName("gameInfo")]
        public JsonDocument GameInfo { get; init; }
        [JsonPropertyName("txTime")]
        public DateTimeOffset TxTime { get; init; }
    }
}
