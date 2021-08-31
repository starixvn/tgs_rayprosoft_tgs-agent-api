using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Shared.Game.Models.WGHorse.Incoming
{
    public record WGHorseGPGameRecordResponse : TgsResponse
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
