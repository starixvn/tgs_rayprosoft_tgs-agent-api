using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models
{
    public record AgentDividendResponse
    {
        [JsonPropertyName("items")]
        public AgentDividend[] Items { get; init; }

        [JsonPropertyName("total")]
        public AgentDividendTotal Total { get; init; }

        [JsonPropertyName("totalcount")]
        public int TotalCount { get; init; }
    }
}
