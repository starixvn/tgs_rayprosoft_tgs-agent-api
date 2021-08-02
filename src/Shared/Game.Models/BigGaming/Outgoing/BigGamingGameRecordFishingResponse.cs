using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingGameRecordFishingResponse
    {
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        [JsonPropertyName("gameType")]
        public int GameType { get; set; }
        [JsonPropertyName("issueId")]
        public string IssueId { get; set; }
        [JsonPropertyName("betId")]
        public string BetId { get; set; }
        [JsonPropertyName("fireCount")]
        public int FireCount { get; set; }
        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; set; }
        [JsonPropertyName("validAmount")]
        public decimal ValidAmount { get; set; }
        [JsonPropertyName("calcAmount")]
        public decimal CalcAmount { get; set; }
        [JsonPropertyName("payout")]
        public decimal Payout { get; set; }
        [JsonPropertyName("orderTime")]
        public DateTime OrderTime { get; set; }
        [JsonPropertyName("jackpot")]
        public string Jackpot { get; set; }
        [JsonPropertyName("jackpotType")]
        public int JackpotType { get; set; }
        [JsonPropertyName("extend")]
        public decimal Extend { get; set; }
        [JsonPropertyName("orderFrom")]
        public string OrderFrom { get; set; }
        [JsonPropertyName("gameBalance")]
        public decimal GameBalance { get; set; }
    }
}
