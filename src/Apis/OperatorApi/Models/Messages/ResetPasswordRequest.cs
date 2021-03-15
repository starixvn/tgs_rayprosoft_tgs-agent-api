using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record ResetPasswordRequest
    {
        public string Token { get; init; }

        [JsonPropertyName("validatecode")]
        public string ValidateCode { get; init; }

        [JsonPropertyName("newpwd")]
        public string NewPassword { get; init; }
    }
}
