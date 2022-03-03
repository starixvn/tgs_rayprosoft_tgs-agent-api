using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models
{
    public record AgentContributionResponse
    {
        [JsonPropertyName("totalcount")]
        public int TotalCount { get; init; }
        [JsonPropertyName("items")]
        public AgentContribution[] Items { get; init; }
        [JsonPropertyName("total")]
        public AgentContributionTotal Total { get; init; }

    }
}
