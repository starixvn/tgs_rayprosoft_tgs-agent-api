using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models.Messages
{
    public record BrandLiveChatModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("setting")]
        public JsonDocument Setting { get; set; }
    }
}