using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingGPGameRecordResponse : TgsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
        [JsonPropertyName("lastGameRecorTime")]
        public DateTimeOffset? LastGameRecorTime { get; init; }
        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }
        [JsonPropertyName("failCount")]
        public int FailCount { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; init; }
    }
}
