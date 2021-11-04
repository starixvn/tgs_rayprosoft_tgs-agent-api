using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.LY.Commons
{
    public record LYGameRecordList
    {
        [JsonPropertyName("GameID")]
        public string[] GameID { get; init; }
        [JsonPropertyName("Accounts")]
        public string[] Accounts { get; init; }
        [JsonPropertyName("ServerID")]
        public int[] ServerID { get; init; }
        [JsonPropertyName("KindID")]
        public int[] KindID { get; init; }
        [JsonPropertyName("TableID")]
        public int[] TableID { get; init; }
        [JsonPropertyName("ChairID")]
        public int?[] ChairID { get; init; }
        [JsonPropertyName("UserCount")]
        public int[] UserCount { get; init; }
        [JsonPropertyName("CellScore")]
        public string[] CellScore { get; init; }
        [JsonPropertyName("AllBet")]
        public string[] AllBet { get; init; }
        [JsonPropertyName("Profit")]
        public string[] Profit { get; init; }
        [JsonPropertyName("Revenue")]
        public string[] Revenue { get; init; }
        [JsonPropertyName("GameStartTime")]
        public string[] GameStartTime { get; init; }
        [JsonPropertyName("GameEndTime")]
        public string[] GameEndTime { get; init; }
        [JsonPropertyName("CardValue")]
        public string[] CardValue { get; init; }
        [JsonPropertyName("ChannelID")]
        public int[] ChannelID { get; init; }
        [JsonPropertyName("LineCode")]
        public string[] LineCode { get; init; }
    }
}
