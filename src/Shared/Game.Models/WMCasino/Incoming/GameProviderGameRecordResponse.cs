using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record GameProviderGameRecordResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("lastGameRecordTime")]
        public DateTimeOffset? LastGameRecordTime { get; init; }
    }
}