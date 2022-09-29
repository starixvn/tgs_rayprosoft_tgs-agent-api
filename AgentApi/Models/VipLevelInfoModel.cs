using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models;

public record VipLevelInfoModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("level")]
    public int Level { get; init; }
}
