using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models;

public record MainSubAgentDividendResponse
{
    [JsonPropertyName("mainitems")] public AgentDividend[] MainItems { get; init; }

    [JsonPropertyName("subitems")] public AgentDividend[] SubItems { get; init; }

    [JsonPropertyName("subtotal")] public AgentDividendTotal SubTotal { get; init; }

    [JsonPropertyName("subtotalcount")] public int SubTotalCount { get; init; }
}