using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRFundCheckingResponse
    {
        [JsonPropertyName("totalRecords")]
        public int TotalRecords { get; set; }
        [JsonPropertyName("recordPage")]
        public int RecordPage { get; set; }
        [JsonPropertyName("recordCountPerPage")]
        public int RecordCountPerPage { get; set; }
        [JsonPropertyName("records")]
        public IEnumerable<VRFundCheckingRecordResponse> Records { get; set; }
    }
}
