using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record BatchResponse
    {
        [JsonPropertyName("error")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("errormsg")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("batchid")]
        public string ExternalBatchId { get; set; }

        [JsonPropertyName("dup")]
        public bool IsDuplicate { get; set; }

        [JsonPropertyName("batchtime")]
        public DateTimeOffset BatchTime { get; set; }

        [JsonPropertyName("transactionscount")]
        public int TransactionsCount { get; set; }

        [JsonPropertyName("playerbalances")]
        public IDictionary<string, decimal> PlayerBalances { get; set; }

        [JsonPropertyName("errors")]
        public BatchResponseTransaction[] Errors { get; set; }
    }
}
