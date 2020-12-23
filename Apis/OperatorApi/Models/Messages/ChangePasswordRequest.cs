using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record ChangePasswordRequest
    {
        [JsonPropertyName("oldpwd")]
        public string OldPassword { get; set; }

        [JsonPropertyName("newpwd")]
        public string NewPassword { get; set; }

        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; set; }
    }
}