using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYGameRecordList
    {
        [JsonPropertyName("GameID")]
        public string[] GameID { get; set; }
        [JsonPropertyName("Accounts")]
        public string[] Accounts { get; set; }
        [JsonPropertyName("ServerID")]
        public int[] ServerID { get; set; }
        [JsonPropertyName("KindID")]
        public int[] KindID { get; set; }
        [JsonPropertyName("TableID")]
        public int[] TableID { get; set; }
        [JsonPropertyName("ChairID")]
        public int[] ChairID { get; set; }
        [JsonPropertyName("UserCount")]
        public int[] UserCount { get; set; }
        [JsonPropertyName("CellScore")]
        public string[] CellScore { get; set; }
        [JsonPropertyName("AllBet")]
        public string[] AllBet { get; set; }
        [JsonPropertyName("Profit")]
        public string[] Profit { get; set; }
        [JsonPropertyName("Revenue")]
        public string[] Revenue { get; set; }
        [JsonPropertyName("GameStartTime")]
        public string[] GameStartTime { get; set; }
        [JsonPropertyName("GameEndTime")]
        public string[] GameEndTime { get; set; }
        [JsonPropertyName("CardValue")]
        public string[] CardValue { get; set; }
        [JsonPropertyName("ChannelID")]
        public int[] ChannelID { get; set; }
        [JsonPropertyName("LineCode")]
        public string[] LineCode { get; set; }
    }
}
