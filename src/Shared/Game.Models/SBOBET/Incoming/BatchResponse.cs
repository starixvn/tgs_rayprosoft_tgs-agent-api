using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record BatchResponse
    {
        [JsonPropertyName("error")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("errormsg")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("batchid")]
        public string ExternalBatchId { get; init; }

        [JsonPropertyName("dup")]
        public bool IsDuplicate { get; init; }

        [JsonPropertyName("batchtime")]
        public DateTimeOffset BatchTime { get; init; }

        [JsonPropertyName("transactionscount")]
        public int TransactionsCount { get; init; }

        [JsonPropertyName("playerbalances")]
        public IDictionary<string, decimal> PlayerBalances { get; init; }

        [JsonPropertyName("errors")]
        public BatchResponseTransaction[] Errors { get; init; }
    }
}