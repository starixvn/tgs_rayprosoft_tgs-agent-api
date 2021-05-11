using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGameRecordResponse
    {
        [JsonPropertyName("recordCountPerPage")]
        public int Rows { get; set; }
        [JsonPropertyName("recordPage")]
        public int Page { get; set; }
        [JsonPropertyName("totalRecords")]
        public int TotalCount { get; set; }
        [JsonPropertyName("betRecords")]
        public VRGameRecordBet[] Records { get; set; }
    }
}
