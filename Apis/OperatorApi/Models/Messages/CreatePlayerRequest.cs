using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record CreatePlayerRequest : BaseUserRequest
    {
        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init;  }

        [JsonPropertyName("pwd")]
        public string Password { get; init; }
        
        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; init; }

        [JsonPropertyName("invitationcode")]
        public string ParentInvitationCode { get; init; }

        [JsonPropertyName("agent")]
        public bool HasAgent { get; init; }

        [JsonPropertyName("testplayer")]
        public bool IsTestPlayer { get; init; } = false;
    }
}
