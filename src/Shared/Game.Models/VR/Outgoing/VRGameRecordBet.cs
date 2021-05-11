using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGameRecordBet
    {
        /// <summary>
        /// 投注訂單號.
        /// </summary>
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }
        /// <summary>
        /// 玩家名称，玩家名称只能使用英文字母和数字，最大长度不能超过 128，英文字母不分大小写。
        /// </summary>
        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }
        /// <summary>
        /// 商戶代碼.
        /// </summary>
        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; set; }
        /// <summary>
        /// 重新颁奖损失，当意外状况发生，导致开奖结
        /// 果错误，此时 VR 会重新颁奖，此参数纪录了
        /// 重新颁奖所带来之损失，精确至小数第三位。
        /// </summary>
        [JsonPropertyName("lossPrize")]
        public decimal LossPrize { get; set; }
        /// <summary>
        /// 商户中奖金额，以 VR 与商户约定好之商户赔率所计算出来的奖金，精确至小数第三位。.
        /// </summary>
        [JsonPropertyName("merchantPrize")]
        public decimal MerchantPrize { get; set; }
        /// <summary>
        /// 玩家中奖金额，以商户设定之玩家赔率所计算出来的玩家奖金，精确至小数第三位。 
        /// </summary>
        [JsonPropertyName("playerPrize")]
        public decimal PlayerPrize { get; set; }
        /// <summary>
        /// 投注状态(0:未颁奖, 1:撤单, 2:未中奖, 3:中奖).
        /// </summary>
        [JsonPropertyName("state")]
        public int State { get; set; }
        /// <summary>
        /// 次级状态，此字段是针对state字段的补充说明，以 bit 表示
        /// (1:玩家撤单, 2:管理员撤单, 4:整期撤单, 8:重新颁奖, 16:和局)
        /// ，亦可同时表示多种状态(如 24:重新颁奖&和局)
        /// </summary>
        [JsonPropertyName("subState")]
        public int SubState { get; set; }
        /// <summary>
        /// 投注单位，为投注接口上让玩家选择的投注单位，
        /// 目前分别为 2 (2 元)、1 (1 元)、0.2 (2 角)、0.1 (1 角)、0.02 (2 分)、0.01 (1 分)、0.002 (2 厘) 、0.001 (1 厘)。
        /// </summary>
        [JsonPropertyName("unit")]
        public decimal Unit { get; set; }
        /// <summary>
        /// 投注倍数.
        /// </summary>
        [JsonPropertyName("multiple")]
        public int Multiple { get; set; }
        /// <summary>
        /// 投注注数.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }
        /// <summary>
        /// 投注金额.
        /// </summary>
        [JsonPropertyName("cost")]
        public decimal Cost { get; set; }
        /// <summary>
        /// 赔率 (于投注接口上显示的奖金数字，有可能是中文，例如香港六合彩).
        /// </summary>
        [JsonPropertyName("odds")]
        public string Odds { get; set; }
        /// <summary>
        /// 玩家赔率(此数值为商户在玩家登入时，利用API 参数代入之数字).
        /// </summary>
        [JsonPropertyName("playerOdds")]
        public int PlayerOdds { get; set; }
        /// <summary>
        /// 投注数字 (内容有可能是中文，例如 VR 百家乐押注闲对。).
        /// </summary>
        [JsonPropertyName("number")]
        public string Number { get; set; }
        /// <summary>
        /// 投注位置 (内容有可能是中文).
        /// </summary>
        [JsonPropertyName("position")]
        public string Position { get; set; }
        /// <summary>
        /// 频道 ID
        /// 请注意:
        /// 以下频道为固定赔率频道
        /// channelId: 15, VR 彩票百家乐
        /// channelId: 76, VR 新百家乐
        /// channelId: 77, VR 龙虎斗
        /// </summary>
        [JsonPropertyName("channelId")]
        public int ChannelId { get; set; }
        /// <summary>
        /// 频道代码.
        /// </summary>
        [JsonPropertyName("channelCode")]
        public string ChannelCode { get; set; }
        /// <summary>
        /// 频道名称 (内容有可能是中文).
        /// </summary>
        [JsonPropertyName("channelName")]
        public string ChannelName { get; set; }
        /// <summary>
        /// 投注种类名称 (内容有可能是中文).
        /// </summary>
        [JsonPropertyName("betTypeName")]
        public string BetTypeName { get; set; }
        /// <summary>
        /// 期号.
        /// </summary>
        [JsonPropertyName("issueNumber")]
        public string IssueNumber { get; set; }
        /// <summary>
        /// 开奖数字.
        /// </summary>
        [JsonPropertyName("winningNumber")]
        public string WinningNumber { get; set; }
        /// <summary>
        /// 备注 (内容有可能是中文).
        /// </summary>
        [JsonPropertyName("note")]
        public string Note { get; set; }
        /// <summary>
        /// 下注时间，UTC(+0)格式:"yyyy-MM-ddTHH:mm:ssZ" 
        /// </summary>
        [JsonPropertyName("createTime")]
        public DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// 最后更新时间(例如: 颁奖, 撤单或重新颁奖的时间)，UTC(+0)格式:"yyyy-MM-ddTHH:mm:ssZ"
        /// </summary>
        [JsonPropertyName("updateTime")]
        public DateTimeOffset? UpdateTime { get; set; }
        /// <summary>
        /// 玩家投注 IP .
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
        /// <summary>
        /// 中奖明细.
        /// </summary>
        [JsonPropertyName("prizeDetail")]
        public VRGameRecordPrizeDetail[] PrizeDetail { get; set; }
    }
}
