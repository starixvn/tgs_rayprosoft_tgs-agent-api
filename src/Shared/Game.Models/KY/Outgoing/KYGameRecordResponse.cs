using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.KY.Commons;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYGameRecordResponse : KYBaseDataResponse
    {
        [JsonPropertyName("start")]
        public long Start { get; set; }
        [JsonPropertyName("end")]
        public long End { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("list")]
        public KYGameRecordList List { get; set; }
    }
}
