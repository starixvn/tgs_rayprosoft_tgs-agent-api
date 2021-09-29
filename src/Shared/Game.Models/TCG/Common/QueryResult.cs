using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record QueryResult
    {
        [JsonPropertyName("ref_no")]
        public string RefNo { get; init; }
        [JsonPropertyName("trans_status")]
        public string TransStatus { get; init; }
    }
}
