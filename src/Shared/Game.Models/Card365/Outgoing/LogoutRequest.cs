using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Outgoing
{
    public record LogoutRequest
    {
        [JsonPropertyName("user")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("cert")]
        public string Cert { get; init; }

        [JsonPropertyName("extension1")]
        public string AgentId { get; init; }
    }
}