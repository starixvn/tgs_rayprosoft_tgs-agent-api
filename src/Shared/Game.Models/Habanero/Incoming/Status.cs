using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record Status
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; } = true;

        [JsonPropertyName("autherror")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HasAuthenticationError { get; init; }

        [JsonPropertyName("message")]
        public string Message { get; init; } = string.Empty;
    }
}