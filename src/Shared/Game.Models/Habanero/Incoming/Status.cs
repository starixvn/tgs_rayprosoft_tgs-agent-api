using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record Status
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("autherror")]
        public bool HasAuthenticationError { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
