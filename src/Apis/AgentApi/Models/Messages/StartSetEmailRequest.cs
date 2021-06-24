using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models.Messages
{
    public record StartSetEmailRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; init; }
    }
}
