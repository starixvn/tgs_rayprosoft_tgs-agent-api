using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGameUrlRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
        [JsonPropertyName("loginTime")]
        public string LoginTime { get; init; }
        /// <summary>
        /// Gets or sets the player odds.
        /// 玩家赔率，为一个大于 0 且小于商户赔率的
        /// 整数。例如: 1960，不填则 VR 平台使用商
        /// 户赔率。
        /// </summary>
        [JsonPropertyName("playerOdds")]
        public int? PlayerOdds { get; init; }
        /// <summary>
        /// Gets or sets the channel identifier.
        /// 频道 ID, 可直接连进频道, 不填或填 0 代表
        /// 进入 VR 默认频道。
        /// </summary>
        [JsonPropertyName("channelId")]
        public int? ChannelId { get; init; }
        /// <summary>
        /// Gets or sets the departure URL.
        /// 来源网址，玩家在 VR 网站退出游戏后会返
        /// 回到此网址(网址需包含 http://或 https://) 
        /// </summary>
        [JsonPropertyName("departureUrl")]
        public string DepartureUrl { get; init; }
        /// <summary>
        /// Gets or sets the is create user.
        /// 检查如果没有玩家账号，若设定为 true 时，
        /// 会先建立玩家账号再登入；若设定为 false
        /// 或是没有设定时，则会登入失败。
        /// </summary>
        [JsonPropertyName("isCreateUser")]
        public string IsCreateUser { get; init; }
        /// <summary>
        /// Gets or sets the wallet deposit URL.
        /// l 商户充值网址，玩家在 VR 页面中会出现链
        /// 接此网址的按钮
        /// </summary>
        [JsonPropertyName("walletDepositUrl")]
        public string WalletDepositUrl { get; init; }
        /// <summary>
        /// Gets or sets the wallet withdraw URL.
        /// 商户提款网址，玩家在 VR 页面中会出现链
        /// 接此网址的按钮
        /// </summary>
        [JsonPropertyName("walletWithdrawUrl")]
        public string WalletWithdrawUrl { get; init; }
        /// <summary>
        /// Gets or sets the wallet URL.
        /// 商户钱包网址，玩家在 VR 页面中会出现链 String Y
        /// 接此网址的按钮
        /// </summary>
        [JsonPropertyName("walletUrl")]
        public string WalletUrl { get; init; }
        /// <summary>
        /// Gets or sets the type of the channel.
        /// 前台选单显示类型
        /// 0: 全部(预设)
        /// 1: 彩票类
        /// 2: 游戏类
        /// 3: 彩票&游戏类
        /// </summary>
        [JsonPropertyName("channelType")]
        public int? ChannelType { get; init; }
    }
}
