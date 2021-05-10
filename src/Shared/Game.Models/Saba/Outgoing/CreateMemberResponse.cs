using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Saba.Enums;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record CreateMemberResponse
    {
        [JsonPropertyName("error_code")]
        public CreateMemberApiError ErrorCode { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; }
    }
}