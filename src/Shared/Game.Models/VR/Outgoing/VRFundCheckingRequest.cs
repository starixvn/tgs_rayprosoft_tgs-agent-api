using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRFundCheckingRequest
    {
        [JsonPropertyName("startTime")]
        public string StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public string endTime { get; init; }
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; init; }
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
        [JsonPropertyName("recordPage")]
        public int RecordPage { get; init; }
        [JsonPropertyName("recordCountPerPage")]
        public int RecordCountPerPage { get; init; }
    }
}
