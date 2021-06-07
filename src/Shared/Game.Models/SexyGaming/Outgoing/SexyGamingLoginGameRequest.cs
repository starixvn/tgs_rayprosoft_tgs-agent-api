using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Common;

namespace TGS.Shared.Game.Models.SexyGaming.Outgoing
{
    public record SexyGamingLoginGameRequest : SexyGamingBaseRequest
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        /// <summary>
        /// 系统会自动检测玩家的装置; 也可明确标示true，false来强制选择玩家是否使用行动版画面.
        /// </summary>
        [JsonPropertyName("isMobileLogin")]
        public bool IsMobileLogin { get; set; }
        /// <summary>
        /// 游戏平台名称.
        /// </summary>
        [JsonPropertyName("platform")]
        public string Platform => "SEXYBCRT";
        /// <summary>
        /// 平台游戏类型.
        /// </summary>
        [JsonPropertyName("gameType")]
        public string GameType => "LIVE";
        /// <summary>
        /// 平台游戏代码.
        /// </summary>
        [JsonPropertyName("gameCode")]
        public string GameCode { get; set; } 
        /// <summary>
        /// Default is English预设为英文.
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; } = "en";
        /// <summary>
        /// Gets the external URL.
        /// </summary>
        [JsonPropertyName("externalURL")]
        private string ExternalURL { get; set; }
        /// <summary>
        /// 擴充參數.
        /// </summary>
        [JsonPropertyName("extension1")]
        public string Extension1 { get; set; }
    }
}
