using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.KY.Commons;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYFundCheckingResponses : KYBaseDataResponse
    {
        /// <summary>
        /// Gets or sets the status.
        /// 状态码（-1:不存在、0:成功、2:失败、3:处理中）
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonPropertyName("status")]
        public int Status { get; init; }

        [JsonPropertyName("money")]
        public decimal? Amount { get; init; }
    }
}
