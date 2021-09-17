using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Outgoing
{
    public record GetKeyRequest
    {
        [JsonPropertyName("cert")]
        public string Cert { get; init; }

        [JsonPropertyName("user")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("userName")]
        public string Username { get; init; }

        [JsonPropertyName("extension1")]
        public string AgentId { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }
    }
}