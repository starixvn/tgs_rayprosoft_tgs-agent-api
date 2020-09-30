using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record ForgotPasswordRequest : BaseRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; init; }
    }
}
