using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGameRecordPrizeDetail
    {
        /// <summary>
        /// 中獎注數.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }
        /// <summary>
        /// 中獎數字(內容有可能是中文).
        /// </summary>
        [JsonPropertyName("number")]
        public string Number { get; set; }
        /// <summary>
        /// 中獎名稱(內容有可能是中文).
        /// </summary>
        [JsonPropertyName("awardName")]
        public string AwardName { get; set; }
    }
}
