using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WGHorse.Incoming
{
    public record WGHorseGPGameRecordRequest
    {
        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; init; }
    }
}
