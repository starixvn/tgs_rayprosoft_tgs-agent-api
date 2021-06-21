using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingVideoRoundQueryItemResponse
    {
        /// <summary>
        /// 游戏状态.
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }
        /// <summary>
        /// 游戏类别.
        /// </summary>
        [JsonPropertyName("gameType")]
        public int GameType { get; set; }
        /// <summary>
        /// 游戏名称.
        /// </summary>
        [JsonPropertyName("gameName")]
        public string GameName { get; set; }
        /// <summary>
        /// 游戏荷官ID.
        /// </summary>
        [JsonPropertyName("dealerId")]
        public int DealerId { get; set; }
        /// <summary>
        /// 游戏荷官.
        /// </summary>
        [JsonPropertyName("dealer")]
        public string Dealer { get; set; }
        /// <summary>
        /// 游戏局号 .
        /// </summary>
        [JsonPropertyName("serialNo")]
        public string SerialNo { get; set; }
        /// <summary>
        /// 桌台ID.
        /// </summary>
        [JsonPropertyName("tableId")]
        public string TableId { get; set; }
        /// <summary>
        /// 百家乐游戏结果.
        /// </summary>
        [JsonPropertyName("baccarat")]
        public string Baccarat { get; set; }
        /// <summary>
        /// 百家乐游戏结果(base64).
        /// </summary>
        [JsonPropertyName("baccarat64")]
        public string Baccarat64 { get; set; }
        /// <summary>
        /// 游戏结果.
        /// </summary>
        [JsonPropertyName("betResult")]
        public string BetResult { get; set; }
        /// <summary>
        /// 游戏结果（原始结果）.
        /// </summary>
        [JsonPropertyName("result")]
        public string Result { get; set; }
        /// <summary>
        /// 开局时间.
        /// </summary>
        [JsonPropertyName("openTime")]
        public string OpenTime { get; set; }
        /// <summary>
        /// 结算时间.
        /// </summary>
        [JsonPropertyName("calcTime")]
        public string CalcTime { get; set; }
    }
}
