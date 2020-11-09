using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record ChangePasswordRequest : BaseTokenRequest
    {
        [Required]
        [JsonPropertyName("oldpwd")]
        public string OldPassword { get; set; }

        [Required]
        [JsonPropertyName("newpwd")]
        public string NewPassword { get; set; }

        [Required]
        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; set; }
    }
}