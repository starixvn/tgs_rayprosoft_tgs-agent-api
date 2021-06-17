using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models.Messages
{
    public record SetPayPasswordRequest
    {
        [JsonPropertyName("pwd")]
        public string Password { get; init; }
    }
}
