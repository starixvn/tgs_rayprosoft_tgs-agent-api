using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public class VRFundCheckingRecordResponse
    {
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        public DateTime createTime { get; set; }
        [JsonPropertyName("state")]
        public int state { get; set; }
        [JsonPropertyName("balance")]
        public decimal balance { get; set; }
    }
}
