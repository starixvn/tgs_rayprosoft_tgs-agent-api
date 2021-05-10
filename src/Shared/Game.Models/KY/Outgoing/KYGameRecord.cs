using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYGameRecord
    {
        [JsonPropertyName("gameID")]
        public string GameID { get; init; }
        [JsonPropertyName("account")]
        public string Account { get; init; }
        [JsonPropertyName("serverID")]
        public int ServerID { get; init; }
        [JsonPropertyName("kindID")]
        public int KindID { get; init; }
        [JsonPropertyName("tableID")]
        public int TableID { get; init; }
        [JsonPropertyName("chairID")]
        public int ChairID { get; init; }
        [JsonPropertyName("userCount")]
        public int UserCount { get; init; }
        [JsonPropertyName("cellScore")]
        public decimal CellScore { get; init; }
        [JsonPropertyName("allBet")]
        public decimal AllBet { get; init; }
        [JsonPropertyName("profit")]
        public decimal Profit { get; init; }
        [JsonPropertyName("revenue")]
        public decimal Revenue { get; init; }
        [JsonPropertyName("gameStartTime")]
        public DateTimeOffset GameStartTime { get; init; }
        [JsonPropertyName("gameEndTime")]
        public DateTimeOffset GameEndTime { get; init; }
        [JsonPropertyName("cardValue")]
        public string CardValue { get; init; }
        [JsonPropertyName("channelID")]
        public int ChannelID { get; init; }
        [JsonPropertyName("lineCode")]
        public string LineCode { get; init; }
    }
}
