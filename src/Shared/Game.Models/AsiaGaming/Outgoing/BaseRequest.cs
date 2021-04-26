using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public abstract record BaseRequest
    {
        [JsonPropertyName("cagent")]
        public string AgentCode { get; init; }

        [JsonPropertyName("loginname")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("actype")]
        public int Actype { get; } = 1;

        [JsonPropertyName("password")]
        public string Password { get; } = "password";

        [JsonPropertyName("oddtype")]
        public string BetLimitId { get; init; }

        [JsonPropertyName("cur")]
        public string Currency { get; init; }
    }
}