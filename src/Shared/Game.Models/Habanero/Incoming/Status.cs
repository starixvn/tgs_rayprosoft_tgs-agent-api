using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record Status
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; } = true;

        [JsonPropertyName("autherror")]
        public bool HasAuthenticationError { get; init; } = false;

        [JsonPropertyName("message")]
        public string Message { get; init; } = string.Empty;
    }
}
