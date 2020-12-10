using System.Text.Json.Serialization;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record PlayerKickRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }
    }
}