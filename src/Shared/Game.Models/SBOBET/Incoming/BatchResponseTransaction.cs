using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record BatchResponseTransaction
    {
        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("txid")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("utxid")]
        public Guid? InternalTransactionId { get; init; }

        [JsonPropertyName("dup")]
        public bool IsDuplicate { get; init; }

        [JsonPropertyName("error")]
        public int Error { get; init; }

        [JsonPropertyName("errormsg")]
        public string ErrorMessage { get; init; }
    }
}