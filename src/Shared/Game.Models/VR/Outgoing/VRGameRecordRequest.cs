using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGameRecordRequest
    {
        [JsonPropertyName("startTime")]
        public string StartTime { get; set; }
        [JsonPropertyName("endTime")]
        public string EndTime { get; set; }
        [JsonPropertyName("channelId")]
        public int ChannelId { get; set; } = -1;
        [JsonPropertyName("issueNumber")]
        public string IssueNumber { get; set; } = "-1";
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; set; } = string.Empty;
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;
        [JsonPropertyName("isUpdateTime")]
        public bool IsUpdateTime { get; set; } = true;
        [JsonPropertyName("state")]
        public int State { get; set; } = -1;
        [JsonPropertyName("betNumberFormat")]
        public int BetNumberFormat { get; set; } = 0;
        [JsonPropertyName("recordPage")]
        public int Page { get; set; } = 0;
        [JsonPropertyName("recordCountPerPage")]
        public int Rows { get; set; } = 1000;
    }
}
