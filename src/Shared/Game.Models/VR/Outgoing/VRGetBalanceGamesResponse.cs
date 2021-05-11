using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGetBalanceGamesResponse
    {
        /// <summary>
        /// 电子游戏种类
        /// </summary>
        [JsonPropertyName("type")]
        public int PlatformType { get; set; }
        /// <summary>
        /// 电子游戏种类名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// 电子游戏钱包余额, 只有当 errorCode 为成功(0)时, 会显示余额, 其他情况都为 0
        /// </summary>
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        /// <summary>
        /// 错误代码
        /// 0: 成功
        /// 1: 尚未建立帐号
        /// 2: 查询余额失败
        /// 3: VR 系统错误
        /// 4: 游戏系统错误
        /// 999: 未知错误
        /// </summary>
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; set; }

    }
}
