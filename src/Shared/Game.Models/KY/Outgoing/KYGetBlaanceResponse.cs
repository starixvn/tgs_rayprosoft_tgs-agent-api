using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.KY.Commons;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYGetBlaanceResponse : KYBaseDataResponse
    {
        /// <summary>
        /// Gets or sets the player code.
        /// 玩家账号
        /// </summary>
        [JsonPropertyName("account")]
        public string PlayerCode { get; set; }
        /// <summary>
        /// Gets or sets the total balance.
        /// 总余额
        /// </summary>
        [JsonPropertyName("totalMoney")]
        public int TotalBalance { get; set; }
        /// <summary>
        /// Gets or sets the balance.
        /// 可下分余额
        /// </summary>
        [JsonPropertyName("freeMoney")]
        public int Balance { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// 状态码（-1、不存在，0、不在线,1、在线，2、封停）
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }
        /// <summary>
        /// Gets or sets the game status.
        /// 0、不在游戏中，1 在游戏中
        /// </summary>
        [JsonPropertyName("gameStatus")]
        public int GameStatus { get; set; }
    }
}
