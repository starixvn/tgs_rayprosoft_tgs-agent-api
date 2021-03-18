using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record BatchRequest
    {
        [JsonPropertyName("batchid")]
        public string ExternalBatchId { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; init; }

        [JsonPropertyName("transactions")]
        public BatchRequestTransaction[] Transactions { get; init; }
    }
}