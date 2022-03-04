using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models
{
    public record AgentContributionResponse
    {
        [JsonPropertyName("items")]
        public AgentContribution[] Items { get; init; }

        [JsonPropertyName("total")]
        public AgentContributionTotal Total { get; init; }

        [JsonPropertyName("totalcount")]
        public int TotalCount { get; init; }
    }
}
