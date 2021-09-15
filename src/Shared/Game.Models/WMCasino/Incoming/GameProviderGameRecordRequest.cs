using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record GameProviderGameRecordRequest
    {
        [JsonPropertyName("brandCode")]
        public string BrandCode { get; init; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("agentId")]
        public string AgentId { get; init; }

        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; init; }

        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; init; }
    }
}