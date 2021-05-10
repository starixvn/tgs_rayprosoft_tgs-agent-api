using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Saba.Enums;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record LoginResponse
    {
        [JsonPropertyName("error_code")]
        public LoginApiError ErrorCode { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("Data")]
        public string Token { get; init; }
    }
}