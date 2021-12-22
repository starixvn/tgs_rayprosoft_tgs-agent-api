using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Incoming
{
    public record JiliAuthorizePlayerRequest
    {
        [JsonPropertyName("token")]
        public string Token { get; init; }

        [JsonPropertyName("reqId")]
        public string RequestId { get; init; }
    }
}
