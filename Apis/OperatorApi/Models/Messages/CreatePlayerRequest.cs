using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record CreatePlayerRequest : BaseUserRequest, IRequest<CreatePlayerResponse>
    {
        [Required]
        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init;  }

        [Required]
        [JsonPropertyName("pwd")]
        public string Password { get; init; }
        
        [Required]
        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; init; }

        [JsonPropertyName("invitationcode")]
        public string ParentInvitationCode { get; init; }

        [JsonPropertyName("agent")]
        public bool HasAgent { get; init; } = false;

        [JsonPropertyName("testplayer")]
        public bool IsTestPlayer { get; init; } = false;
    }
}
