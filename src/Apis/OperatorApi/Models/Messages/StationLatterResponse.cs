using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record StationLatterResponse
    {
        [JsonPropertyName("createon")]
        public DateTimeOffset CreateOn { get; init; }

        [JsonPropertyName("subject")]
        public string Subject { get; init; }

        [JsonPropertyName("content")]
        public string Content { get; init; }
    }
}
