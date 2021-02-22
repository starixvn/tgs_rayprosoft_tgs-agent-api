using System;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record BatchRequest : IRequest<BatchResponse>
    {
        [JsonPropertyName("batchid")]
        public string ExternalBatchId { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; init; }

        [JsonPropertyName("transactions")]
        public BatchRequestTransaction[] Transactions { get; init; }
    }
}
