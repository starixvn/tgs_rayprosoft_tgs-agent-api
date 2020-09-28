using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record CreatePlayerRequest
    {
        [JsonPropertyName("answer")]
        public string Answer { get; init; }

        [JsonPropertyName("captcha")]
        public string Captcha { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("password")]
        public string Password { get; init; }

        [JsonPropertyName("confirmpwd")]
        public string ConfirmPassword { get; init; }

        [JsonPropertyName("parentinvitationcode")]
        public string ParentInvitationCode { get; init; }

        [JsonPropertyName("isagent")]
        public bool HasAgent { get; init; }

        [JsonPropertyName("istestplayer")]
        public bool IsTestPlayer { get; init; }
    }
}
