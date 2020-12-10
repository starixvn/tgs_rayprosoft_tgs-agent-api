using System;
using System.Text.Json.Serialization;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record GetRoundHistoryRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("roundid")]
        public Guid RoundId { get; init; }

        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}