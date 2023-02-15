using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

#nullable enable

namespace TGS.Apis.AgentApi.Models;

public class PlayerInfoModel
{
    [JsonPropertyName("areacode")]
    public string? AreaCode { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("playercode")]
    public string PlayerCode { get; init; } = default!;

    [JsonPropertyName("registeredtime")]
    public DateTimeOffset? RegisteredTime { get; init; }

    [JsonPropertyName("registerhost")]
    public string? RegisterHost { get; init; }

    [JsonPropertyName("status")]
    public PlayerStatus Status { get; init; }

    [JsonPropertyName("username")]
    public string Username { get; init; } = default!;

    [JsonPropertyName("viplevel")]
    public int? VipLevel { get; init; }
}
