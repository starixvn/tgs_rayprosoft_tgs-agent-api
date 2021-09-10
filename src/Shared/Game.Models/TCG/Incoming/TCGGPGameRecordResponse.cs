using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGGPGameRecordResponse : TgsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
        [JsonPropertyName("lastGameRecorDirectory")]
        public string LastGameRecorDirectory { get; set; }
        [JsonPropertyName("lastGameRecorFile")]
        public string LastGameRecorFile { get; set; }
        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }
        [JsonPropertyName("failCount")]
        public int FailCount { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; init; }
    }
}
