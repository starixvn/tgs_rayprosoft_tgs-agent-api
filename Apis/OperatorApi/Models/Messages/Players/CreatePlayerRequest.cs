using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record CreatePlayerRequest : BaseRequest
    {
        [Required]
        [JsonPropertyName("answer")]
        public string Answer { get; init; }

        [Required]
        [JsonPropertyName("captcha")]
        public string Captcha { get; init; }

        [Required]
        [JsonPropertyName("pwd")]
        public string Password { get; init; }
        
        [Required]
        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; init; }

        [JsonPropertyName("parentinvitationcode")]
        public string ParentInvitationCode { get; init; }

        [Required]
        [JsonPropertyName("isagent")]
        public bool HasAgent { get; init; }

        [Required]
        [JsonPropertyName("istestplayer")]
        public bool IsTestPlayer { get; init; }
    }
}
