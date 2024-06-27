using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models;

public class AgentModel
{
    [JsonPropertyName("id")] public Guid AgnetId { get; set; }

    [JsonPropertyName("balance")] public decimal Balance { get; set; }

    /// <summary>
    /// 佔成
    /// </summary>
    [JsonPropertyName("dividendpercentage")]
    public decimal DividendPercentage { get; set; }

    [JsonPropertyName("registeredon")] public DateTimeOffset RegisteredOn { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; }

    [JsonPropertyName("teamnumber")] public int TeamNumber { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }
}