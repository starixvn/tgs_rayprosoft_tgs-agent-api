using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record CreatePlayerRequest : BaseUserRequest
    {
        [Required]
        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init;  }

        [Required]
        [JsonPropertyName("lang")]
        public string Language { get; init; }

        [Required]
        [JsonPropertyName("pwd")]
        public string Password { get; init; }
        
        [Required]
        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; init; }

        [JsonPropertyName("invitationcode")]
        public string ParentInvitationCode { get; init; }

        [Required]
        [JsonPropertyName("agent")]
        public bool HasAgent { get; init; }

        [JsonPropertyName("testplayer")]
        public bool IsTestPlayer { get; init; } = false;
    }
}
