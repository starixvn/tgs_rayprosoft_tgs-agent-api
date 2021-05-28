using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Common;
using TGS.Shared.Game.Models.SexyGaming.Enums;

namespace TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions
{
    public record SexyGamingBet
    {
        [JsonPropertyName("platformTxId")]
        public string PlatformTxId { get; set; }
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("currency")]
        public SexyGamingCurrencyCode Currency { get; set; }
        [JsonPropertyName("platform")]
        public SexyGamingPlatform Platform { get; set; }
        [JsonPropertyName("gameType")]
        public SexyGamingGameType GameType { get; set; }
        [JsonPropertyName("gameCode")]
        public string GameCode { get; set; }
        [JsonPropertyName("gameName")]
        public string GameName { get; set; }
        [JsonPropertyName("betType")]
        public string BetType { get; set; }
        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; set; }
        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; set; }
        [JsonPropertyName("roundId")]
        public string RoundId { get; set; }
        [JsonPropertyName("gameInfo")]
        public JsonDocument GameInfo { get; set; }
    }
}
