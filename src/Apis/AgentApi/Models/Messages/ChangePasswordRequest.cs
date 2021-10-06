using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models.Messages
{
    public record ChangePasswordRequest
    {
        [Required]
        [JsonPropertyName("oldpwd")]
        public string OldPassword { get; set; }

        [Required]
        [JsonPropertyName("newpwd")]
        public string NewPassword { get; set; }
    }
}
