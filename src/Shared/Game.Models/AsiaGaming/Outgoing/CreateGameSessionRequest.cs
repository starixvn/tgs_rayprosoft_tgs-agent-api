using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public record CreateGameSessionRequest
    {
        [JsonPropertyName("productid")]
        public string AgentCode { get; init; }

        [JsonPropertyName("username")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("session_token")]
        public Guid LoginRecordId { get; init; }

        [JsonPropertyName("credit")]
        public decimal Balance { get; init; }
    }
}