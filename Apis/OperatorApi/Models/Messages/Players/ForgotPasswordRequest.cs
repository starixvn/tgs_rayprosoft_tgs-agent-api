using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record ForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; init; }
    }
}
