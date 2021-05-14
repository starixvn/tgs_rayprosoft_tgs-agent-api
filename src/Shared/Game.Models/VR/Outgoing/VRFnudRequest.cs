using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRFnudRequest
    {
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; init; }
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
        /// <summary>
        /// 转点类型(0:入款，1:提款).
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; init; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
        [JsonPropertyName("createTime")]
        public string CreateTime { get; init; }
    }
}
